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
using PagedList;
using TimeOffTracker.Model.Enum;
using TimeOffTracker.Services;

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
        private MailNotification _mailNotification;

        public EmployeeController(MailNotification mailNotification)
        {
            _mailNotification = mailNotification;
        }

        /// <summary>
        /// Создает новую заявку на отпуск
        /// POST: /Employee/СreateRequest
        /// Header
        /// {
        ///     Authorization: Bearer {TOKEN}
        /// }
        /// 
        /// Перед использованием, проверять коллизию дат на другие заявки /Employee/CheckDateCollision
        /// </summary>
        /// <param name="requestDto">
        ///  * Меченые поля указываются если requestTypeId == (1, 2, 4)
        /// Форматы даты со временем: "2021-07-24T10:07:57.237Z"
        /// Body
        /// {
        ///     "dateTimeFrom": "2021-07-25",
        ///     "dateTimeTo": "2021-07-26",
        ///     "requestTypeId": 1,     
        ///     "reason": "string",
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
            requestDto.UserSignature ??= new List<UserSignatureDto>();

            var requestCrud = new TimeOffTracker.CRUD.Request();
            var result = await requestCrud.CheckRequestAsync(requestDto, token);
            if (result != "Ok")
            {
                return BadRequest(result);
            }

            var requestId = await requestCrud.CreateAsync(requestDto, token);

            /*
                Отсылаем уведомление на почту
                бухгалтерией, соответствующая информация высылается другим менеджерам по порядку, если надо.
            */

            // SendMaill

            return requestId;
        }

        /// <summary>
        /// Проверка дат на коллизию с другими заявками
        /// POST: /Employee/CheckDateCollision
        /// Header
        /// {
        ///     Authorization: Bearer {TOKEN}
        /// }
        /// </summary> 
        /// </summary>
        /// <param name="requestDto">
        /// {
        ///     "dateTimeFrom": "2021-08-02",
        ///     "dateTimeTo": "2021-08-08"
        /// }
        /// </param>
        /// <param name="token"></param>
        /// <returns></returns>
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(404)]
        [HttpPost]
        public async Task<ActionResult<string>> CheckDateCollision([FromBody] RequestDto requestDto,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var userIdStr = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var userId = int.Parse(userIdStr);
            requestDto.UserId = userId;

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
        /// <param name="requestDto">
        ///  * Меченые поля указываются если requestTypeId == (1, 2, 4)
        /// Форматы даты со временем: "2021-07-24T10:07:57.237Z"
        /// {
        ///     "id": "25",
        ///     "dateTimeFrom": "2021-08-01",
        ///     "dateTimeTo": "2021-08-02",
        ///     "requestTypeId": 1,
        ///     "reason": "string",
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
            requestDto.UserId = userId;

            var requestRepository = new RequestRepository();
            var request = await requestRepository.SelectByIdAndUserIdAsync(requestDto.Id, userId, token);
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

            requestDto.StateDetailId = StateDetails.New;

            var requestCrud = new TimeOffTracker.CRUD.Request();
            var result = await requestCrud.CheckRequestAsync(requestDto, token);
            if (result != "Ok")
            {
                return BadRequest(result);
            }

            await requestCrud.UpdateAsync(requestDto, token);

            // SendMaill

            return Ok(requestDto.Id);
        }

        /// <summary>
        /// Изменить сотрудников заявки в состояние “В процессе”
        /// PUT: /Employee/EditInProgressRequest
        /// Header
        /// {
        ///     Authorization: Bearer {TOKEN}
        /// }
        /// </summary>
        /// <param name="requestDto">
        /// Body
        /// {
        ///     "id": "25",
        ///     "userSignatureDto": [
        ///         {
        ///             "nInQueue": 0,
        ///             "userId": 4
        ///         },
        ///         {
        ///             "nInQueue": 1,
        ///             "userId": 9
        ///         },
        ///         {
        ///             "nInQueue": 2,
        ///             "userId": 13
        ///         }
        ///     ]
        /// }
        /// </param>
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
            requestDto.UserId = userId;

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
            var result = await requestCrud.CheckManagersAsync(requestDto.UserSignature, token);
            if (!result)
            {
                return BadRequest("Wrong Manager set");
            }

            var userSignatureRepository = new UserSignatureRepository();

            await userSignatureRepository.DeleteAllNotApprovedAsync(requestDto.Id, token);
            await requestCrud.AddUserSignatureAsync(requestDto.UserSignature, requestDto.Id, 0, token);

            // SendMaill

            return Ok(requestDto.Id);
        }

        /// <summary>
        /// Редактирование одобренной заявки
        /// PUT: /Employee/EditApprovedRequest
        /// Header
        /// {
        ///     Authorization: Bearer {TOKEN}
        /// }
        /// 
        /// </summary>
        /// <param name="requestDto">
        ///  * Меченые поля указываются если requestTypeId == (1, 2, 4)
        /// Форматы даты со временем: "2021-07-24T10:07:57.237Z"
        /// Body
        /// {
        ///     "id": "25", 
        ///     "dateTimeFrom": "2021-07-25",
        ///     "dateTimeTo": "2021-07-26",
        ///     "requestTypeId": 1,     
        ///     "reason": "string",
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
        [HttpPut]
        public async Task<ActionResult<int>> EditApprovedRequest([FromBody] RequestDto requestDto,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var userIdStr = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var userId = int.Parse(userIdStr);
            requestDto.UserId = userId;

            var requestRepository = new RequestRepository();
            var request = await requestRepository.SelectByIdAndUserIdAsync(requestDto.Id, userId, token);
            if (request == null)
            {
                return NoContent();
            }

            var enumRepository = new EnumRepository();
            if (request.StateDetailId != (int) StateDetails.Approved)
            {
                var state = enumRepository.GetById<StateDetails>(request.StateDetailId);
                return BadRequest($"Request.State: {state.Type}");
            }

            var requestCrud = new TimeOffTracker.CRUD.Request();
            var result = await requestCrud.CheckRequestAsync(requestDto, token);
            if (result != "Ok")
            {
                return BadRequest(result);
            }

            requestDto.StateDetailId = StateDetails.ModifiedByOwner;
            await requestCrud.UpdateAsync(requestDto, token);

            requestDto.Id = 0;
            requestDto.StateDetailId = StateDetails.New;
            var requestId = await requestCrud.CreateAsync(requestDto, token);

            // SendMaill

            return Ok(requestId);
        }

        /// <summary>
        /// Удалить заявку со статусом New
        /// DELETE: /Employee/DeleteNewRequest?id=12
        /// Header
        /// {
        ///     Authorization: Bearer {TOKEN}
        /// }
        /// </summary>
        /// <param name="id">Ид заявки</param>
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
            await requestCrud.DeleteOwnerAsync(id, token);
            return Ok("Ok");
        }

        /// <summary>
        /// Удалить заявку со статусом InProgress или Approved
        /// DELETE: /Employee/DeleteInProgressOrApprovedRequest?id=12
        /// Header
        /// {
        ///     Authorization: Bearer {TOKEN}
        /// }
        /// </summary>
        /// <param name="id">Ид заявки</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(404)]
        [HttpDelete]
        public async Task<ActionResult<string>> DeleteInProgressOrApprovedRequest([FromQuery(Name = "id")] int id,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var userIdStr = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var userId = int.Parse(userIdStr);

            var requestRepository = new RequestRepository();
            var request = await requestRepository.SelectAsync(id, userId, token);
            if (request == null)
            {
                return NoContent();
            }

            var enumRepository = new EnumRepository();
            if (request.StateDetailId != (int) StateDetails.InProgress &&
                request.StateDetailId != (int) StateDetails.Approved)
            {
                var state = enumRepository.GetById<StateDetails>(request.StateDetailId);
                return BadRequest($"Request.State: {state.Type}");
            }

            var requestCrud = new TimeOffTracker.CRUD.Request();
            await requestCrud.DeleteOwnerAsync(id, token);

            //SendEmaill
            //8. Все люди уже утвердившие заявку получают соответствующее уведомление на почту,
            //как  и в случае отмены заявки в середины цепочки.

            return Ok("Ok");
        }

        /// <summary>
        /// Получить список менеджеров
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

        [ProducesResponseType(200, Type = typeof(List<UserDto>))]
        [ProducesResponseType(404)]
        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetAccounting(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var userRepository = new UserRepository();
            var managers = await userRepository.SelectAllByRoleAsync(UserRoles.Accounting, token);
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
        /// Получить детальную информацию о заявке
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
        ///          "userSignatures": [us1, us2, us3]
        ///      }
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
            var request1 = await requestRepository.SelectByIdAndUserIdAsync(id, userId, token);
            if (request1 == null)
            {
                return base.NoContent();
            }

            var request = await requestRepository.SelectFullAsync(id, token);
            var requestDto = Converter.EntityToDto(request);
            return Ok(requestDto);
        }

        /// <summary>
        /// POST: /Employee/GetRequests?page=3&pageSize=10
        /// Header
        /// {
        ///     Authorization: Bearer {TOKEN}
        /// }
        /// </summary>
        /// <param name="filter">
        /// Получить все заявки
        /// Body
        /// {
        ///     "RequestTypeId": 0,
        ///     "StateDetailId": 0,
        ///     "Reason": ""
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
            filter.UserId = userId;

            var requestRepository = new RequestRepository();

            var requests = await requestRepository.SelectAllAsync(filter, token);

            var totalPages = (int) Math.Ceiling((double) requests.Count / pageSize);
            var requestsDto = requests.ToPagedList(page, pageSize).Select(Converter.EntityToDto);
            var result = new
            {
                page = page,
                pageSize = pageSize,
                totalPages = totalPages,
                requests = requestsDto,
            };
            return Ok(result);
        }

        /// <summary>
        /// Вернет кличество дней, каждого типа отпуска, со статусом Approved.
        /// С начала года, которые получил пользователь 
        /// GET: /Employee/GetDays
        /// Header
        /// {
        ///     Authorization: Bearer {TOKEN}
        /// }
        /// </summary>
        /// <param name="token"></param>
        /// <returns>
        /// {
        ///     "1": {"days": 3, "hours": 0, "minutes": 0},
        ///     "2": {"days": 9, "hours": 0, "minutes": 0},
        ///     "3": {"days": 3, "hours": 0, "minutes": 0},
        ///     "4": {"days": 3, "hours": 0, "minutes": 0},
        ///     "5": {"days": 3, "hours": 0, "minutes": 0},
        ///     "6": {"days": 3, "hours": 0, "minutes": 0},
        ///     "7": {"days": 3, "hours": 0, "minutes": 0}
        /// }
        /// </returns>
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(404)]
        [HttpGet]
        public async Task<ActionResult<Dictionary<int, TimeSpan>>> GetDays(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var userIdStr = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var userId = int.Parse(userIdStr);

            var requestRepository = new RequestRepository();

            var enumRepository = new EnumRepository();
            var requestTypes = enumRepository.GetAll<RequestTypes>();
            var days = new Dictionary<int, TimeSpan>();
            foreach (var rt in requestTypes)
            {
                var timeSpan = await requestRepository.GetDays(userId, rt.Id, token);
                days.Add(rt.Id, timeSpan);
            }

            return Ok(days);
        }
    }
}