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
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(404)]
        [HttpPost]
        public async Task<ActionResult<int>> СreateRequest([FromBody] RequestDto request, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            request.UserSignatureDto ??= new List<UserSignatureDto>();

            //Получить текущего пользователя
            var userId = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            request.UserId = int.Parse(userId);
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

            var userRepository = new UserRepository();
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
                request.UserSignatureDto = new List<UserSignatureDto>();
            }

            //Проверка менеджеров на наличие в базе
            var checkPass = await userRepository.CheckManagers(request.UserSignatureDto, token);
            if (!checkPass)
            {
                return BadRequest("Wrong Manager set");
            }

            var accounting = await userRepository.SelectOneAccounting(token);
            if (accounting == null)
            {
                return BadRequest("Accounting not found");
            }

            var requestRepository = new RequestRepository();
            var requestId = await requestRepository.InsertAsync(request, token);

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
                бухгалтерией, соответствующая информация высылается другим менеджерам по порядку, если надо.
            */

            return requestId;
        }

        [HttpGet]
        void GetManagers()
        {
        }


        [HttpPut]
        void EditRequest(RequestDto requestDto)
        {
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
        }

        [HttpPost]
        int CopyRequest(int id)
        {
            /*
             3. Сотрудник может дублировать уже существующую заявку на отпуск в любом статусе.
            Сценарии использования:Дублирование заявки на отпуск
                1. Пользователь входит в систему “Отпуск”, используя свои доменные логин-пароль.
                2. Пользователь видит список своих заявок.
                3. Пользователь выбирает заявку в  любом и нажимает  Просмотреть (View). 
                4. Открывается страница с деталями заявки. 
                5. Пользователь нажимает Дублировать (Duplicate).
                6. Открывается страница создания новой заявки, заполненная информацией из дублируемой заявки.
            */
            return new RequestDto().Id;
        }

        [HttpGet]
        List<RequestDto> GetRequests(RequestDto filter = null)
        {
            /*
             Статистика заявок
                1. Сотрудник может просмотреть статус любой своей  заявки на отпуск.
                2. Сотрудник может просмотреть  количество использованных дней отпуска каждого типа в году.  
            //	Если фильтр filter == null то вернуть всех
            //	иначе вернуть похожие по указаному фильтру
            //	выводить старницами, по 10 елементов на страницу
            */
            return new List<RequestDto>();
        }

        [HttpGet]
        RequestDto GetRequestDetails(int id)
        {
            //	детальная информация о заявке
            return new RequestDto();
        }

        [HttpDelete]
        void DeleteRequest(int request_id)
        {
            /*
             Сценарии использования: Отмена полностью утвержденной заявки или заявки в процессе утверждения
                1. Пользователь входит в систему “Отпуск”, используя свои доменные логин-пароль.
                2. Пользователь видит список своих заявок.
                3. Пользователь выбирает заявку в состоянии “Утверждена” (Approved) или “В процессе” (In progress), у которой конечная дата (To) позже текущей даты, и нажимает Просмотреть (View). 
                4. Открывается страница с деталями заявки. 
                5. Пользователь нажимает Отменить (Decline).
                6. Появляется сообщение: “Do you really want to decline the approved request?” (“Вы действительно хотите изменить утвержденную заявку?”)
                7. Если пользователь нажимает Да, заявка переходит в состояние Отменена (Rejected). В причине отмены заявки:”Declined by the owner” (“Отменена сотрудником”) 
                8. Все люди уже утвердившие заявку получают соответствующее уведомление на почту, как  и в случае отмены заявки в середины цепочки.
            */
            //if request.StateDetailId == 1, New
            //	request.StateDetailId = 5 Deleted Заявка была Удалена пользователем до первой подписи 
        }


        [HttpPut]
        void EditRequestUserSignature(int requestId, List<UserSignatureDto> users)
        {
        }

        [HttpGet]
        void GetDays()
        {
            //	Получить количество дней которые полил работник з начала года до текушей даты
        }
        //	Наверно нужно создать еще свои пользовательские ошибки?
        //  И кидать когда нужно, что бы потом обработчик ошибок в midelwares их записал в лог
        //  и выдал потом нужный Http статус?

        /*
        2.Пользователь выбрал даты, которые пересекаются с уже существующей заявкой на отпуск (утвержденной или в процессе)
            Сообщение:”There is another request for these dates. Do you really want to create one more request?” 
            (“Есть другая  заявка на эти даты. Вы хотите создать еще одну заявку?”)
            Yes (Да)  - заявка создается
            No (Нет) - заявка не создается
            
        3.Пользователь оставил незаполненным одно из обязательных полей
            Сообщение:”This field is required” (“Это обязательное поле”)
            Заявка не создается.
        */
    }
}