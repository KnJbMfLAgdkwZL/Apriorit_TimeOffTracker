using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TimeOffTracker.Model;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using TimeOffTracker.Model.DTO;
using TimeOffTracker.Model.Repositories;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Query;
using PagedList;
using TimeOffTracker.Model.Enum;

namespace TimeOffTracker.Controllers
{
    /// <summary>
    /// Сотрудник (Employee): 
    ///     ◦ Может оставлять заявку на отпуск
    ///     ◦ Может просмотреть статистику своих заявок на отпуск
    /// Менеджер (Manager):
    ///     ◦ Может оставлять заявки на отпуск, как обычный сотрудник 
    ///     ◦ Может просмотреть статистику своих заявок на отпуск
    /// </summary>
    [Route("[controller]/[action]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize(Roles = "Employee, Manager")]
    public class EmployeeController : ControllerBase
    {
        /// <summary>
        /// POST: /Employee/СreateRequest
        /// Header
        /// {
        ///     Authorization: Bearer {TOKEN}
        /// }
        /// </summary>
        /// <param name="requestDto">
        ///  * Меченые поля указываются если requestTypeId == (1, 2, 4)
        /// Форматы даты со временем: "2021-07-24T10:07:57.237Z"
        /// 
        /// Body
        /// {
        ///     "dateTimeFrom": "2021-07-25",
        ///     "dateTimeTo": "2021-07-26",
        /// 
        ///     "requestTypeId": 1,     
        ///     "reason": "string",
        /// 
        ///  *  "projectRoleComment": "Варил кофе",
        ///  *  "projectRoleTypeId": 1, 
        ///  *  "userSignatureDto": [
        ///         {
        ///             "nInQueue": 0,
        ///             "userId": 4
        ///         }
        ///     ]
        /// }
        /// </param>
        /// <param name="token"></param>
        /// <returns>
        /// request.Id
        /// </returns>
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(404)]
        [HttpPost]
        public async Task<ActionResult<int>> СreateRequest([FromBody] RequestDto requestDto, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            //Получить текущего пользователя
            var userIdStr = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var userId = int.Parse(userIdStr);

            requestDto.UserId = userId;

            //Если список подписчиков не задан, создаем пустой лист
            requestDto.UserSignatureDto ??= new List<UserSignatureDto>();

            var requestCrud = new TimeOffTracker.CRUD.Request();
            var chek = await requestCrud.ChekAsync(requestDto, token);
            if (chek != "Ok")
            {
                return BadRequest(chek);
            }

            var requestId = await requestCrud.CreateAsync(requestDto, token);

            /*
                Отсылаем уведомление на почту
                бухгалтерией, соответствующая информация высылается другим менеджерам по порядку, если надо.
            */

            // SendMaill

            return requestId;
        }

        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(404)]
        [HttpPost]
        public async Task<ActionResult<string>> CheckDateCollision([FromBody] RequestDto requestDto,
            CancellationToken token)
        {
            var requestRepository = new RequestRepository();
            var request = await requestRepository.CheckDateCollision(requestDto, token);
            if (request != null)
            {
                return BadRequest("There is another request for these dates.");
            }

            return Ok("Ok");
        }

        /// <summary>
        /// PUT: /Employee/EditNewRequest
        /// Header
        /// {
        ///     Authorization: Bearer {TOKEN}
        /// }
        /// </summary>
        /// <param name="requestDto"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(404)]
        [HttpPut]
        public async Task<ActionResult<int>> EditNewRequest([FromBody] RequestDto requestDto, CancellationToken token)
        {
            //До первого утверждения сотрудник может полностью изменить заявку на отпуск

            token.ThrowIfCancellationRequested();
            var userIdStr = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var userId = int.Parse(userIdStr);

            var requestRepository = new RequestRepository();
            var request = await requestRepository.SelectByIdAndUserIdAsync(requestDto.Id, userId, token);
            if (request is not {UserSignatures: {Count: > 0}})
            {
                return NoContent();
            }

            var enumRepository = new EnumRepository();
            if (request.StateDetailId != (int) StateDetails.New)
            {
                var state = enumRepository.GetById<StateDetails>(request.StateDetailId);
                return BadRequest($"Request.State: {state.Type}");
            }

            var requestCrud = new TimeOffTracker.CRUD.Request();
            var result = await requestCrud.ChekAsync(requestDto, token);
            if (result != "Ok")
            {
                return BadRequest(result);
            }

            await requestCrud.UpdateAsync(requestDto, token);

            // SendMaill

            return Ok();
        }

        /// <summary>
        /// PUT: /Employee/EditInProgressRequest
        /// Header
        /// {
        ///     Authorization: Bearer {TOKEN}
        /// }
        /// </summary>
        /// <param name="requestDto"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(404)]
        [HttpPut]
        public async Task<ActionResult<int>> EditInProgressRequest([FromBody] RequestDto requestDto,
            CancellationToken token)
        {
            //Изменение частично утвержденной заявки (состояние “В процессе”)
            //После первого утверждения, но раньше финального утверждения,
            //сотрудник может изменить человека подписывающего отпуск в списке еще не подписавших.

            token.ThrowIfCancellationRequested();
            var userIdStr = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var userId = int.Parse(userIdStr);

            var requestRepository = new RequestRepository();
            var request = await requestRepository.SelectByIdAndUserIdAsync(requestDto.Id, userId, token);
            if (request == null)
            {
                return NoContent();
            }

            var enumRepository = new EnumRepository();
            if (request.StateDetailId != (int) StateDetails.InProgress)
            {
                var state = enumRepository.GetById<StateDetails>(request.StateDetailId);
                return BadRequest($"Request.State: {state.Type}");
            }

            var requestCrud = new TimeOffTracker.CRUD.Request();
            var checkPass = await requestCrud.CheckManagersAsync(requestDto.UserSignatureDto, token);
            if (!checkPass)
            {
                return BadRequest("Wrong Manager set");
            }

            var userSignatureRepository = new UserSignatureRepository();

            await userSignatureRepository.DeleteAllNotApprovedAsync(requestDto.Id, token);
            await requestCrud.AddUserSignature(requestDto.UserSignatureDto, requestDto.Id, 0, token);

            // SendMaill

            return Ok(requestDto.Id);
        }

        /*
                
            Изменение полностью утвержденной заявки (состояние  “Утверждена”)
                4. После финального утверждения сотрудник может  изменить даты заявки. 
                В этом случае заявка должна пройти снова утверждение у всех ответственных лиц, как при начальном утверждении.
                
                3. Пользователь выбирает заявку в состоянии “Утверждена” (Approved), у которой конечная дата (To) позже текущей даты, и нажимает Просмотреть (View). 
                6. Появляется сообщение: “Do you really want to edit the approved request?” (“Вы действительно хотите изменить утвержденную заявку?”)
                7. Если пользователь нажимает Да, заявка открывается для редактирования. 
                8. Пользователь может поменять:
                    a. Даты
                    b. Причину
                    c. Людей подтверждающих заявку 
                9. После того, как пользователь нажал Сохранить:
                    a. Старая заявка переходит в состояние Отменена (Rejected). В причине отмены заявки:”Modified by the owner” (“Изменена сотрудником”) 
                    b. В системе появляется новая заявка, которая должна пройти новый цикл утверждения.
            */

        /// <summary>
        /// DELETE: /Employee/DeleteNewRequest?id=12
        /// Header
        /// {
        ///     Authorization: Bearer {TOKEN}
        /// }
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(404)]
        [HttpDelete]
        public async Task<ActionResult<string>> DeleteNewRequest([FromQuery(Name = "id")] int id,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var userIdStr = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var userId = int.Parse(userIdStr);

            var requestRepository = new RequestRepository();
            var request = await requestRepository.SelectByIdAndUserIdAsync(id, userId, token);
            if (request == null)
            {
                return NoContent();
            }

            var enumRepository = new EnumRepository();
            if (request.StateDetailId != (int) StateDetails.New)
            {
                var state = enumRepository.GetById<StateDetails>(request.StateDetailId);
                return BadRequest($"Request.State: {state.Type}");
            }

            var requestCrud = new TimeOffTracker.CRUD.Request();
            await requestCrud.DeleteOwner(id, token);
            return Ok();
        }

        /*
            Изменение заявки на отпуск
                3. После финального утверждения сотрудник может отменить заявку на отпуск. 
                В этом случае отмену заявки подтверждает только бухгалтерия.
                
            Сценарии использования: Отмена полностью утвержденной заявки или заявки в процессе утверждения
                3. Пользователь выбирает заявку в состоянии 
                    “Утверждена” (Approved) или 
                    “В процессе” (In progress), 
                    у которой конечная дата (To) позже текущей даты, и нажимает Просмотреть (View). 
                4. Открывается страница с деталями заявки. 
                5. Пользователь нажимает Отменить (Decline).
                6. Появляется сообщение: “Do you really want to decline the approved request?” 
                (“Вы действительно хотите изменить утвержденную заявку?”)
                7. Если пользователь нажимает Да, заявка переходит в состояние Отменена (Rejected). 
                В причине отмены заявки:”Declined by the owner” (“Отменена сотрудником”) 
                8. Все люди уже утвердившие заявку получают соответствующее уведомление на почту, как  и в случае отмены заявки в середины цепочки.
            */

        /// <summary>
        /// GET: /Employee/GetManagers
        /// Header
        /// {
        ///     Authorization: Bearer {TOKEN}
        /// }
        /// </summary>
        /// <returns>
        /// [user1, user2, user3]
        /// </returns>
        [ProducesResponseType(200, Type = typeof(List<UserDto>))]
        [ProducesResponseType(404)]
        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetManagers(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var userRepository = new UserRepository();
            var managers = await userRepository.SelectAllByRoleAsync(UserRoles.Manager, token);
            var managersDto = new List<UserDto>();
            foreach (var mDto in managers.Select(Converter.EntityToDto))
            {
                mDto.Login = "";
                mDto.Password = "";
                managersDto.Add(mDto);
            }

            return Ok(managersDto);
        }

        ///  <summary>
        ///  GET: /Employee/GetRequestDetails?id=10
        ///  Header
        ///  {
        ///      Authorization: Bearer {TOKEN}
        ///  }
        ///  </summary>
        ///  <param name="id">ИД заявки</param>
        ///  <param name="token"></param>
        ///  <returns>
        ///  Body
        ///  {
        ///      "request": {
        ///          "id": 12,
        ///          "requestTypeId": 1,
        ///          "reason": "string",
        ///          "projectRoleComment": "string",
        ///          "projectRoleTypeId": 1,
        ///          "userId": 3,
        ///          "stateDetailId": 1,
        ///          "dateTimeFrom": "2021-07-25T00:00:00",
        ///          "dateTimeTo": "2021-07-26T00:00:00",
        ///          "userSignatureDto": null
        ///      },
        ///      "userSignatures": [us1, us2, us3]
        /// }
        ///  </returns>
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(404)]
        [HttpGet]
        public async Task<ActionResult<string>> GetRequestDetails([FromQuery(Name = "id")] int id,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var userIdStr = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var userId = int.Parse(userIdStr);

            var requestRepository = new RequestRepository();
            var request = await requestRepository.SelectByIdAndUserIdAsync(id, userId, token);
            if (request == null)
            {
                return base.NoContent();
            }

            return Ok(new
            {
                Request = Converter.EntityToDto(request),
                UserSignatures = request.UserSignatures is {Count: > 0}
                    ? request.UserSignatures.Select(Converter.EntityToDto).ToList()
                    : new List<UserSignatureDto>()
            });
        }

        /*/// <summary>
        /// PUT: /Employee/EditRequestUserSignature
        /// Header
        /// {
        ///     Authorization: Bearer {TOKEN}
        /// }
        /// </summary>
        /// <param name="id"></param>
        /// <param name="users"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(404)]
        [HttpPut]
        public async Task<ActionResult<int>> EditRequestUserSignature([FromQuery(Name = "id")] int id,
            [FromBody] List<UserSignatureDto> users,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var userIdStr = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var userId = int.Parse(userIdStr);

            return Ok();
        }*/

        /*/// <summary>
        /// POST: /Employee/GetRequests?page=3&pageSize=10
        /// Header
        /// {
        ///     Authorization: Bearer {TOKEN}
        /// }
        /// </summary>
        /// <param name="filter">
        /// Body
        /// {
        ///     "RequestTypes": "1",
        ///     "StateDetails": "1",
        ///     "DateTime": "",
        ///     "DateTime": ""
        /// }
        /// </param>
        /// <param name="page">Текущая страница</param>
        /// <param name="pageSize">Размер страницы</param>
        /// <param name="token"></param>
        /// <returns>
        /// {
        ///     "page": 3,
        ///     "pageSize": 2,
        ///     "totalPages": 12,
        ///     "users": [Request1, Request2, Request3]
        /// }
        /// </returns>
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(404)]
        [HttpPost]
        public async Task<ActionResult<string>> GetRequests([FromBody] RequestDto filter,
            [FromQuery(Name = "page")] int page,
            [FromQuery(Name = "pageSize")] int pageSize, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var userIdStr = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var userId = int.Parse(userIdStr);

            /*
             Статистика заявок
                1. Сотрудник может просмотреть статус любой своей  заявки на отпуск.  
            //	Если фильтр filter == null то вернуть всех
            //	иначе вернуть похожие по указаному фильтру
            //	выводить старницами, по 10 елементов на страницу
            #1#
            return Ok();
        }*/

        /*
        /// <summary>
        /// GET: /Employee/GetDays
        /// Header
        /// {
        ///     Authorization: Bearer {TOKEN}
        /// }
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(404)]
        [HttpGet]
        public async Task<ActionResult<int>> GetDays(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var userIdStr = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var userId = int.Parse(userIdStr);

            /*
                Сотрудник может просмотреть  количество использованных дней отпуска каждого типа в году.
             #1#
            return Ok();
        }*/
    }
}