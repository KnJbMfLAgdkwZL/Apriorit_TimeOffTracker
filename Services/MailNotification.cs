using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using TimeOffTracker.Model;
using TimeOffTracker.Model.Enum;
using TimeOffTracker.Model.Repositories;
using TimeOffTracker.Services.EmailService;

namespace TimeOffTracker.Services
{
    public class MailNotification
    {
        private IEmailService _mail;

        public MailNotification(IEmailService mail)
        {
            _mail = mail;
        }

        /// <summary>
        /// Утверждение заявки
        /// Такое письмо последовательно получают все люди, указанные в списке ответственных за утверждение заявки.
        /// </summary>
        public void SendRequest(Request request)
        {
            var enumRepository = new EnumRepository();
            var requestType = enumRepository.GetById<RequestTypes>(request.RequestTypeId);
            var projectRoleType = enumRepository.GetById<ProjectRoleTypes>(request.ProjectRoleTypeId);

            //var r = request.UserSignatures.Where(us => us.Approved == true).ToList();


            var subject =
                $"Запрос на отпуск. Заявка {request.User.SecondName} {request.User.FirstName} [{request.Reason}] <{requestType.Type}> ({request.DateTimeFrom} - {request.DateTimeTo})";

            var body = $@"Запрос на отпуск!
    Причина заявки: {request.Reason}
    Тип заявки: {requestType.Type}
    Причина заявки: {request.Reason}
    Дата начала: {request.DateTimeFrom}
    Дата конца : {request.DateTimeTo}
    Роль: {projectRoleType.Type}
    Комментарии: {request.ProjectRoleComment}
    Кто: {request.User.SecondName} {request.User.FirstName}
    Подписавшие: {request.UserSignatures}
    Approve | Reject";

            Console.WriteLine(subject);
            Console.WriteLine(body);

            foreach (var us in request.UserSignatures)
            {
                Console.WriteLine(us.User.Id);
            }

            var mailAddress = new MailAddress(request.User.Email);
            
            //_mail.SendEmail(mailAddress, subject, body);
        }

        /// <summary>
        /// Нотификация об утвержденной заявке
        ///	Такое письмо получает человек оставивший заявку, когда заявка полностью утверждена.
        /// </summary>
        public void ApprovedRequestEmployee(Request request)
        {
            var enumRepository = new EnumRepository();
            var requestType = enumRepository.GetById<RequestTypes>(request.RequestTypeId);
            var projectRoleType = enumRepository.GetById<ProjectRoleTypes>(request.ProjectRoleTypeId);

            var subject =
                $"Ваша Заявка Утверждена [{request.Reason}] <{requestType.Type}> ({request.DateTimeFrom} - {request.DateTimeTo})";

            var body =
                $"Ваша Заявка Утверждена!" +
                $"Причина заявки: {request.Reason}" +
                $"Тип: {requestType.Type}" +
                $"Даты: {request.DateTimeFrom} - {request.DateTimeTo}" +
                $"Тип заявки: {requestType.Type}" +
                $"Причина заявки: {request.Reason}" +
                $"Дата начала: {request.DateTimeFrom}" +
                $"Дата конца: {request.DateTimeTo}" +
                $"Роль: {projectRoleType.Type}" +
                $"Комментарии: {request.ProjectRoleComment}" +
                $"Кто: {request.User.SecondName} {request.User.FirstName}" +
                $"Подписавшие: {request.UserSignatures}";

            var mailAddress = new MailAddress(request.User.Email);
            _mail.SendEmail(mailAddress, subject, body);
        }

        /// <summary>
        /// Нотификация об утвержденной заявке
        /// Такое письмо получает бухгалтерия:
        /// </summary>
        public void ApprovedRequestAccounting(Request request)
        {
            var enumRepository = new EnumRepository();
            var requestType = enumRepository.GetById<RequestTypes>(request.RequestTypeId);
            var projectRoleType = enumRepository.GetById<ProjectRoleTypes>(request.ProjectRoleTypeId);

            var subject =
                $"Заявка Утверждена {request.User.SecondName} {request.User.FirstName} [{request.Reason}] <{requestType.Type}> ({request.DateTimeFrom} - {request.DateTimeTo})";

            var body =
                $"Заявка Утверждена!" +
                $"Причина заявки: {request.Reason}" +
                $"Тип: {requestType.Type}" +
                $"Даты: {request.DateTimeFrom} - {request.DateTimeTo}" +
                $"Тип заявки: {requestType.Type}" +
                $"Причина заявки: {request.Reason}" +
                $"Дата начала: {request.DateTimeFrom}" +
                $"Дата конца: {request.DateTimeTo}" +
                $"Роль: {projectRoleType.Type}" +
                $"Комментарии: {request.ProjectRoleComment}" +
                $"Кто: {request.User.SecondName} {request.User.FirstName}" +
                $"Подписавшие: {request.UserSignatures}";

            //список всех утвердивших заявку через запятую
            //Получить 0 с типом бугалтер отправить ему письмо
        }

        /// <summary>
        /// Нотификация об отклоненной заявке
        /// Такое письмо получает человек оставивший заявку, когда заявка отклонена не им самим.
        /// </summary>
        public void RejectedRequestEmployee(Request request)
        {
            var enumRepository = new EnumRepository();
            var requestType = enumRepository.GetById<RequestTypes>(request.RequestTypeId);
            var projectRoleType = enumRepository.GetById<ProjectRoleTypes>(request.ProjectRoleTypeId);

            var subject =
                $"Ваша Заявка Отклонена [{request.Reason}] <{requestType.Type}> ({request.DateTimeFrom} - {request.DateTimeTo})";

            var body =
                $"Ваша Заявка Отклонена!" +
                $"Причина заявки: {request.Reason}" +
                $"Тип: {requestType.Type}" +
                $"Даты: {request.DateTimeFrom} - {request.DateTimeTo}" +
                $"Тип заявки: {requestType.Type}" +
                $"Причина заявки: {request.Reason}" +
                $"Дата начала: {request.DateTimeFrom}" +
                $"Дата конца: {request.DateTimeTo}" +
                $"Роль: {projectRoleType.Type}" +
                $"Комментарии: {request.ProjectRoleComment}" +
                $"Кто: {request.User.SecondName} {request.User.FirstName}" +
                $"Подписавшие: {request.UserSignatures}" +
                $"Отклонена: {request.UserSignatures}" +
                $"Причина отклонения: <указанная причина отклонения заявки>" +
                $"Вы можете учесть замечания и оставить новую заявку.";


            var mailAddress = new MailAddress(request.User.Email);
            _mail.SendEmail(mailAddress, subject, body);
        }

        /// <summary>
        /// Нотификация об отклоненной заявке
        /// Такое письмо получает бухгалтерия и все люди, уже утвердившие заявку.
        /// </summary>
        public void RejectedRequestAccountingAndMangers(Request request)
        {
            var enumRepository = new EnumRepository();
            var requestType = enumRepository.GetById<RequestTypes>(request.RequestTypeId);
            var projectRoleType = enumRepository.GetById<ProjectRoleTypes>(request.ProjectRoleTypeId);

            var subject =
                $"Заявка Отклонена {request.User.SecondName} {request.User.FirstName} [{request.Reason}] <{requestType.Type}> ({request.DateTimeFrom} - {request.DateTimeTo})";

            var body =
                $"Заявка Отклонена!" +
                $"Причина заявки: {request.Reason}" +
                $"Тип: {requestType.Type}" +
                $"Даты: {request.DateTimeFrom} - {request.DateTimeTo}" +
                $"Тип заявки: {requestType.Type}" +
                $"Причина заявки: {request.Reason}" +
                $"Дата начала: {request.DateTimeFrom}" +
                $"Дата конца: {request.DateTimeTo}" +
                $"Роль: {projectRoleType.Type}" +
                $"Комментарии: {request.ProjectRoleComment}" +
                $"Кто: {request.User.SecondName} {request.User.FirstName}" +
                $"Подписавшие: {request.UserSignatures}" +
                $"Отклонена: {request.UserSignatures}" +
                $"Причина отклонения: <указанная причина отклонения заявки>";
        }
    }
}