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
            var stateDetailType = enumRepository.GetById<StateDetails>(request.StateDetailId);

            var approved = request.UserSignatures.Where(us => us.Approved == true)
                .Select(us => us.User.SecondName + " " + us.User.FirstName).ToList();

            var waiting = request.UserSignatures.Where(us => us.Approved == false)
                .Select(us => us.User.SecondName + " " + us.User.FirstName).ToList();

            var appr = string.Join(", ", approved);
            var wait = string.Join(", ", waiting);

            var subject =
                $"Запрос на отпуск. Заявка {request.User.SecondName} {request.User.FirstName} [{request.Reason}] <{requestType.Type}> ({request.DateTimeFrom} - {request.DateTimeTo})";

            const string n = " <br/> ";

            var body =
                $@"Запрос на отпуск! {n}
                Кто: {request.User.SecondName} {request.User.FirstName} {n}
                Статус заявки: {stateDetailType.Type} {n}
                Причина заявки: {request.Reason} {n}
                Тип заявки: {requestType.Type} {n}
                Дата начала: {request.DateTimeFrom} {n}
                Дата конца : {request.DateTimeTo} {n}
                Роль на проекте: {projectRoleType.Type} {n}
                Обязанности: {request.ProjectRoleComment} {n}
                Подписавшие: {appr} {n}
                Должны подписать: {wait} {n}
                Approve | Reject";

            Console.WriteLine(subject);
            Console.WriteLine(body);

            //var mailAddress = new MailAddress(request.User.Email);
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
            var stateDetailType = enumRepository.GetById<StateDetails>(request.StateDetailId);

            var approved = request.UserSignatures.Where(us => us.Approved == true)
                .Select(us => us.User.SecondName + " " + us.User.FirstName).ToList();

            var waiting = request.UserSignatures.Where(us => us.Approved == false)
                .Select(us => us.User.SecondName + " " + us.User.FirstName).ToList();

            var appr = string.Join(", ", approved);
            var wait = string.Join(", ", waiting);

            var subject =
                $"Ваша Заявка Утверждена [{request.Reason}] <{requestType.Type}> ({request.DateTimeFrom} - {request.DateTimeTo})";

            const string n = " <br/> ";

            var body =
                $@"Ваша Заявка Утверждена! {n}
                Кто: {request.User.SecondName} {request.User.FirstName} {n}
                Статус заявки: {stateDetailType.Type} {n}
                Причина заявки: {request.Reason} {n}
                Тип заявки: {requestType.Type} {n}
                Дата начала: {request.DateTimeFrom} {n}
                Дата конца : {request.DateTimeTo} {n}
                Роль на проекте: {projectRoleType.Type} {n}
                Обязанности: {request.ProjectRoleComment} {n}
                Подписавшие: {appr} {n}
                Должны подписать: {wait} {n}";

            //var mailAddress = new MailAddress(request.User.Email);
            //_mail.SendEmail(mailAddress, subject, body);
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
            var stateDetailType = enumRepository.GetById<StateDetails>(request.StateDetailId);

            var approved = request.UserSignatures.Where(us => us.Approved == true)
                .Select(us => us.User.SecondName + " " + us.User.FirstName).ToList();

            var waiting = request.UserSignatures.Where(us => us.Approved == false)
                .Select(us => us.User.SecondName + " " + us.User.FirstName).ToList();

            var appr = string.Join(", ", approved);
            var wait = string.Join(", ", waiting);

            var subject =
                $"Заявка Утверждена {request.User.SecondName} {request.User.FirstName} [{request.Reason}] <{requestType.Type}> ({request.DateTimeFrom} - {request.DateTimeTo})";

            const string n = " <br/> ";

            var body =
                $@"Заявка Утверждена! {n}
                Кто: {request.User.SecondName} {request.User.FirstName} {n}
                Статус заявки: {stateDetailType.Type} {n}
                Причина заявки: {request.Reason} {n}
                Тип заявки: {requestType.Type} {n}
                Дата начала: {request.DateTimeFrom} {n}
                Дата конца : {request.DateTimeTo} {n}
                Роль на проекте: {projectRoleType.Type} {n}
                Обязанности: {request.ProjectRoleComment} {n}
                Подписавшие: {appr} {n}
                Должны подписать: {wait} {n}";

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
            var stateDetailType = enumRepository.GetById<StateDetails>(request.StateDetailId);

            var approved = request.UserSignatures.Where(us => us.Approved == true)
                .Select(us => us.User.SecondName + " " + us.User.FirstName).ToList();

            var waiting = request.UserSignatures.Where(us => us.Approved == false)
                .Select(us => us.User.SecondName + " " + us.User.FirstName).ToList();

            var rejectedUs = request.UserSignatures.Where(us => us.Approved == false && us.Reason.Length > 0).ToList();
            var rejected = rejectedUs.Select(us => us.User.SecondName + " " + us.User.FirstName).ToList();

            var appr = string.Join(", ", approved);
            var wait = string.Join(", ", waiting);
            var reje = string.Join(", ", rejected);
            var reason = rejectedUs.FirstOrDefault()?.Reason;

            var subject =
                $"Ваша Заявка Отклонена [{request.Reason}] <{requestType.Type}> ({request.DateTimeFrom} - {request.DateTimeTo})";

            const string n = " <br/> ";

            var body =
                $@"Ваша Заявка Отклонена! {n}
                Кто: {request.User.SecondName} {request.User.FirstName} {n}
                Статус заявки: {stateDetailType.Type} {n}
                Причина заявки: {request.Reason} {n}
                Тип заявки: {requestType.Type} {n}
                Дата начала: {request.DateTimeFrom} {n}
                Дата конца : {request.DateTimeTo} {n}
                Роль на проекте: {projectRoleType.Type} {n}
                Обязанности: {request.ProjectRoleComment} {n}
                Подписавшие: {appr} {n}
                Должны подписать: {wait} {n}
                Отклонена: {reje} {n}
                Причина отклонения: {reason} {n}
                Вы можете учесть замечания и оставить новую заявку.";


            //var mailAddress = new MailAddress(request.User.Email);
            //_mail.SendEmail(mailAddress, subject, body);
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
            var stateDetailType = enumRepository.GetById<StateDetails>(request.StateDetailId);

            var approved = request.UserSignatures.Where(us => us.Approved == true)
                .Select(us => us.User.SecondName + " " + us.User.FirstName).ToList();

            var waiting = request.UserSignatures.Where(us => us.Approved == false)
                .Select(us => us.User.SecondName + " " + us.User.FirstName).ToList();

            var rejectedUs = request.UserSignatures.Where(us => us.Approved == false && us.Reason.Length > 0).ToList();
            var rejected = rejectedUs.Select(us => us.User.SecondName + " " + us.User.FirstName).ToList();

            var appr = string.Join(", ", approved);
            var wait = string.Join(", ", waiting);
            var reje = string.Join(", ", rejected);
            var reason = rejectedUs.FirstOrDefault()?.Reason;

            const string n = " <br/> ";

            var subject =
                $"Заявка Отклонена {request.User.SecondName} {request.User.FirstName} [{request.Reason}] <{requestType.Type}> ({request.DateTimeFrom} - {request.DateTimeTo})";

            var body =
                $@"Заявка Отклонена! {n}
                Кто: {request.User.SecondName} {request.User.FirstName} {n}
                Статус заявки: {stateDetailType.Type} {n}
                Причина заявки: {request.Reason} {n}
                Тип заявки: {requestType.Type} {n}
                Дата начала: {request.DateTimeFrom} {n}
                Дата конца : {request.DateTimeTo} {n}
                Роль на проекте: {projectRoleType.Type} {n}
                Обязанности: {request.ProjectRoleComment} {n}
                Подписавшие: {appr} {n}
                Должны подписать: {wait} {n}
                Отклонена: {reje} {n}
                Причина отклонения: {reason} {n}";
            
        }
    }
}