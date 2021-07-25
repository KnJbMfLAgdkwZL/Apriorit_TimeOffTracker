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
        /// <param name="request">
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
        public async Task<ActionResult<int>> СreateRequest([FromBody] RequestDto request, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            //Получить текущего пользователя
            var userId = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            request.UserId = int.Parse(userId);

            //Если список подписчиков не задан, создаем пустой лист
            request.UserSignatureDto ??= new List<UserSignatureDto>();

            //Статус заявки
            request.StateDetailId = StateDetails.New;

            //Проверка даты
            var time1 = request.DateTimeFrom.Ticks - DateTime.Now.Ticks;
            var time2 = request.DateTimeTo.Ticks - DateTime.Now.Ticks;
            var time3 = request.DateTimeTo.Ticks - request.DateTimeFrom.Ticks;
            if (time1 < 0 || time2 < 0)
            {
                return BadRequest("The dates are in the past. Please change the dates");
            }

            if (time3 < 0)
            {
                return BadRequest("Start date is greater than end date");
            }

            /*
             Нужно добавить проверку на пересечение дат
             
             Пользователь выбрал даты, которые пересекаются с уже существующей заявкой на отпуск (утвержденной или в процессе)
             Сообщение:”There is another request for these dates. Do you really want to create one more request?”
             (“Есть другая  заявка на эти даты. Вы хотите создать еще одну заявку?”)
             Yes (Да)  - заявка создается
             No (Нет) - заявка не создается
             */

            //Проверка причины отпуска на пустоту
            if (string.IsNullOrEmpty(request.Reason))
            {
                return BadRequest("Reason not set");
            }

            var enumRepository = new EnumRepository();

            //Проверка типа отпуска
            if (!enumRepository.Contains(request.RequestTypeId))
            {
                return BadRequest("Wrong RequestType");
            }

            //Типы отпуска который должны подписать менеджеры
            var managers = new List<RequestTypes>()
            {
                RequestTypes.PaidLeave,
                RequestTypes.AdministrativeUnpaidLeave,
                RequestTypes.StudyLeave
            };
            if (managers.Contains(request.RequestTypeId))
            {
                //Менеджер не был указан
                if (request.UserSignatureDto.Count <= 0)
                {
                    return BadRequest("Manager not set");
                }

                //Не указан тип участия в проекте
                if (!enumRepository.Contains(request.ProjectRoleTypeId))
                {
                    return BadRequest("Wrong ProjectRoleType");
                }

                //Не указны обязанности на проекте
                if (string.IsNullOrEmpty(request.ProjectRoleComment))
                {
                    return BadRequest("ProjectRoleComment not set");
                }
            }

            //Типы отпуска который должна подписать только бухгалтерия
            var accountingOnly = new List<RequestTypes>()
            {
                RequestTypes.AdministrativeUnpaidLeave,
                RequestTypes.SocialLeave,
                RequestTypes.SickLeaveWithDocuments,
                RequestTypes.SickLeaveWithoutDocuments
            };
            if (accountingOnly.Contains(request.RequestTypeId))
            {
                //Обнуляем список подписчиков
                request.UserSignatureDto = new List<UserSignatureDto>();
            }

            var userRepository = new UserRepository();

            //Проверка менеджеров на наличие в базе
            var checkPass = await userRepository.CheckManagers(request.UserSignatureDto, token);
            if (!checkPass)
            {
                return BadRequest("Wrong Manager set");
            }

            //Находим бухгалтерию
            var accounting = await userRepository.SelectOneAccounting(token);
            if (accounting == null)
            {
                return BadRequest("Accounting not found");
            }

            //Создаем заявку
            var requestRepository = new RequestRepository();
            var requestId = await requestRepository.InsertAsync(request, token);

            //Добавляем бухгалтерию в список подписчиков
            request.UserSignatureDto.Add(new UserSignatureDto()
            {
                NInQueue = -1,
                RequestId = requestId,
                UserId = accounting.Id
            });

            var userSignatureRepository = new UserSignatureRepository();

            //Убираем менеджеров которые повторяются
            request.UserSignatureDto = request.UserSignatureDto
                .GroupBy(car => car.UserId)
                .Select(g => g.First())
                .ToList();

            //Сортируем по NInQueue
            request.UserSignatureDto.Sort((x, y) => x.NInQueue.CompareTo(y.NInQueue));

            //Выстраиваем в правильном порядке и добавляем подписчиков в бд
            var nInQueue = 0;
            foreach (var us in request.UserSignatureDto)
            {
                us.NInQueue = nInQueue;
                us.RequestId = requestId;
                us.Approved = false;
                us.Deleted = false;

                nInQueue++;

                await userSignatureRepository.InsertAsync(us, token);
            }

            /*
                Отсылаем уведомление на почту
                бухгалтерией, соответствующая информация высылается другим менеджерам по порядку, если надо.
            */

            return requestId;
        }

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

        /// <summary>
        /// PUT: /Employee/EditRequest
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
        public async Task<ActionResult<int>> EditRequest([FromBody] RequestDto request, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var userIdStr = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var userId = int.Parse(userIdStr);

            /*
             Изменение заявки на отпуск
                1. До первого утверждения сотрудник может полностью изменить заявку на отпуск или удалить ее.
                2. После первого утверждения, но раньше финального утверждения, сотрудник может изменить человека подписывающего отпуск в списке еще не подписавших.
                3. После финального утверждения сотрудник может отменить заявку на отпуск. В этом случае отмену заявки подтверждает только бухгалтерия. 
                4. После финального утверждения сотрудник может  изменить даты заявки. В этом случае заявка должна пройти снова утверждение у всех ответственных лиц, как при начальном утверждении.
                
            Сценарии использования: Изменение заявки на отпуск
            Изменение неутвержденной заявки (состояние “Новая”)
                1. Пользователь входит в систему “Отпуск”, используя свои доменные логин-пароль.
                2. Пользователь видит список своих заявок.
                3. Пользователь выбирает заявку в состоянии “Новая” (New) и нажимает  Просмотреть (View). 
                4. Открывается страница с деталями заявки. 
                5. Пользователь нажимает Редактировать (Edit).
                6. Заявка становится доступна для полного редактирования (изменить можно все). После изменения в бухгалтерию отправляется повторное письмо с заявкой. 
    
            Изменение частично утвержденной заявки (состояние “В процессе”)
                1. Пользователь входит в систему “Отпуск”, используя свои доменные логин-пароль.
                2. Пользователь видит список своих заявок.
                3. Пользователь выбирает заявку в состоянии “В процессе” (In progress) и нажимает Просмотреть (View). 
                4. Открывается страница с деталями заявки. 
                5. Пользователь нажимает Редактировать (Edit).
                6. Заявка позволяет изменить людей, еще не подписавших заявку. После изменения возможно одно из двух:
                    a. Изменен следующий человек в цепочке: Новому менеджеру отправляется письмо.
                    b. Изменен человек, который не должен еще подписывать заявку (не следующий за последним подписавшим): Не происходит никаких дополнительных действий.
            
            Изменение полностью утвержденной заявки (состояние  “Утверждена”)
                1. Пользователь входит в систему “Отпуск”, используя свои доменные логин-пароль.
                2. Пользователь видит список своих заявок.
                3. Пользователь выбирает заявку в состоянии “Утверждена” (Approved), у которой конечная дата (To) позже текущей даты, и нажимает Просмотреть (View). 
                4. Открывается страница с деталями заявки. 
                5. Пользователь нажимает Редактировать (Edit).
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
            return Ok();
        }

        /// <summary>
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
        }

        /// <summary>
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
            */
            return Ok();
        }

        /// <summary>
        /// DELETE: /Employee/DeleteRequest
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
        public async Task<ActionResult<string>> DeleteRequest([FromQuery(Name = "id")] int id, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var userIdStr = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var userId = int.Parse(userIdStr);

            /*
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
            //if request.StateDetailId == 1, New
            //	request.StateDetailId = 5 Deleted Заявка была Удалена пользователем до первой подписи
            return Ok();
        }

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
             */
            return Ok();
        }
        //	Наверно нужно создать еще свои пользовательские ошибки?
        //  И кидать когда нужно, что бы потом обработчик ошибок в midelwares их записал в лог
        //  и выдал потом нужный Http статус?
    }
}