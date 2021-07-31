using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TimeOffTracker.Model;
using Microsoft.AspNetCore.Authorization;
using TimeOffTracker.Model.DTO;
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
    /// Бухгалтерия (Accounting): Роль пользователя Бухгалтерия - это обобщенная роль для нескольких людей.  
    ///     ◦ Первым подписывает заявку на любой отпуск
    ///     ◦ Получает нотификацию о полностью подписанной заявке 
    ///     ◦ Может в любой момент просмотреть статистику заявок
    /// Менеджер (Manager):
    ///     ◦ Подписывает заявку на отпуск
    ///     ◦ Может в любой момент
    ///     ◦ просмотреть статистику заявок
    /// </summary>
    [Route("[controller]/[action]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize(Roles = "Manager, Accounting")]
    public class ManagerController : ControllerBase
    {
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(404)]
        [HttpGet]
        public async Task<ActionResult<string>> AcceptRequest([FromQuery(Name = "id")] int id, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var userIdStr = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var userId = int.Parse(userIdStr);

            var userSignatureRepository = new UserSignatureRepository();
            var userSignature = await userSignatureRepository.SelectOneAsync(userId, id, token);

            var requestRepository = new RequestRepository();
            var request = await requestRepository.SelectAsync(id, token);

            if (userSignature == null || request == null)
            {
                return NoContent();
            }

            if (userSignature.NInQueue != 0)
            {
                return BadRequest("Not your turn");
            }

            if (userSignature.Approved)
            {
                return BadRequest("Already approved");
            }

            if (request.StateDetailId != (int) StateDetails.New &&
                request.StateDetailId != (int) StateDetails.InProgress)
            {
                var enumRepository = new EnumRepository();
                var state = enumRepository.GetById<StateDetails>(request.StateDetailId);
                return BadRequest($"Request.StateDetail: {state.Type}");
            }

            await userSignatureRepository.ConfirmSignatureAsync(userSignature, token);

            //Send email next manager

            return Ok("Ok");
        }

        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(404)]
        [HttpGet]
        public async Task<ActionResult<string>> RejectRequest(
            [FromQuery(Name = "id")] int id,
            [FromQuery(Name = "reason")] string reason,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var userIdStr = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var userId = int.Parse(userIdStr);

            var userSignatureRepository = new UserSignatureRepository();
            var userSignature = await userSignatureRepository.SelectOneAsync(userId, id, token);

            var requestRepository = new RequestRepository();
            var request = await requestRepository.SelectAsync(id, token);

            if (userSignature == null || request == null)
            {
                return NoContent();
            }

            if (userSignature.NInQueue != 0)
            {
                return BadRequest("Not your turn");
            }

            if (userSignature.Approved)
            {
                return BadRequest("Already approved");
            }

            if (request.StateDetailId != (int) StateDetails.New &&
                request.StateDetailId != (int) StateDetails.InProgress)
            {
                var enumRepository = new EnumRepository();
                var state = enumRepository.GetById<StateDetails>(request.StateDetailId);
                return BadRequest($"Request.StateDetail: {state.Type}");
            }

            if (string.IsNullOrEmpty(reason))
            {
                return BadRequest("Reason not set");
            }

            userSignature.Reason = reason;
            await userSignatureRepository.UpdateAsync(userSignature, token);

            request.StateDetailId = (int) StateDetails.Rejected;
            await requestRepository.UpdateAsync(request, token);

            //Send email to Accountant about Rejecting

            return Ok("Ok");
        }

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

            //	Получить по фильтру
            //	Менеджер или Бугалтерия видит список заявок, требующих их утверждения.
            return Ok();
        }

        [HttpGet]
        Request GetRequestDetails(int id)
        {
            return new Request();
        }
        /*
         Ошибки
        Менеджера и бугалтерии YES
            1.Пользователь пытается повторно утвердить заявку из письма
                В открывшейся системе “Отпуск”, пользователь получает  сообщение:
                ”The request has already been approved!” (“Заявка уже успешно утверждена!”)
            
            2.Пользователя убрали из списка людей, которые должны утвердить заявку, и пользователь 
            пытается утвердить заявку из письма.  Или же роль пользователя сменили с Менеджера на
            Сотрудник и он пытается утвердить заявку из письма.
                В открывшейся системе “Отпуск”, пользователь получает  сообщение:
                ”The request is not actual!” (“Заявка не актуальна!”)
            
            3.Пользователь не залогинен в систему Отпуск и пытается утвердить заявку из письма.
                Сперва открывается страница логина, после корректного ввода логин/пароля пользователь получает страницу
                с сообщением, что заявка утверждена.
                
        Менеджера И бугалтерии NO
            1. Пользователь пытается отклонить/подтвердить отклоненную заявку из письма
                В открывшейся системе “Отпуск”, пользователь получает  сообщение:”The request has already been rejected!” (“Заявка уже отклонена!”)
            2. Пользователя убрали из списка людей, которые должны утвердить заявку, и пользователь пытается отклонить заявку из письма. Или роль изменена на Сотрудник  и пользователь пытается отклонить заявку
                В открывшейся системе “Отпуск”, пользователь получает  сообщение:”The request is not actual!” (“Заявка не актуальна!”)
            3. Пользователь не залогинен в систему Отпуск и пытается отменить заявку из письма.
                Сперва открывается страница логина, после корректного ввода логин/пароля пользователь получает страницу для ввода причины отмены заявки. .
         */
    }
}