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

            var body =
                $@"
                Запрос на отпуск!
                Кто: {request.User.SecondName} {request.User.FirstName}
                Статус заявки: {stateDetailType.Type}
                Причина заявки: {request.Reason}
                Тип заявки: {requestType.Type}
                Дата начала: {request.DateTimeFrom}
                Дата конца : {request.DateTimeTo}
                Роль на проекте: {projectRoleType.Type}
                Обязанности: {request.ProjectRoleComment}
                Подписавшие: {appr}
                Должны подписать: {wait}

                Approve | Reject";

            var userSignature = request.UserSignatures
                .FirstOrDefault(us => us.Approved == false && us.NInQueue == 0);

            if (userSignature == null) return;

            var mailAddress = new MailAddress(userSignature.User.Email);
            _mail.SendEmail(mailAddress, subject, body);
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

            var body =
                $@"
                Ваша Заявка Утверждена!
                Кто: {request.User.SecondName} {request.User.FirstName}
                Статус заявки: {stateDetailType.Type}
                Причина заявки: {request.Reason}
                Тип заявки: {requestType.Type}
                Дата начала: {request.DateTimeFrom}
                Дата конца : {request.DateTimeTo}
                Роль на проекте: {projectRoleType.Type}
                Обязанности: {request.ProjectRoleComment}
                Подписавшие: {appr}
                Должны подписать: {wait}";

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
            var stateDetailType = enumRepository.GetById<StateDetails>(request.StateDetailId);

            var approved = request.UserSignatures.Where(us => us.Approved == true)
                .Select(us => us.User.SecondName + " " + us.User.FirstName).ToList();

            var waiting = request.UserSignatures.Where(us => us.Approved == false)
                .Select(us => us.User.SecondName + " " + us.User.FirstName).ToList();

            var appr = string.Join(", ", approved);
            var wait = string.Join(", ", waiting);

            var subject =
                $"Заявка Утверждена {request.User.SecondName} {request.User.FirstName} [{request.Reason}] <{requestType.Type}> ({request.DateTimeFrom} - {request.DateTimeTo})";

            var body =
                $@"
                Заявка Утверждена!
                Кто: {request.User.SecondName} {request.User.FirstName}
                Статус заявки: {stateDetailType.Type}
                Причина заявки: {request.Reason}
                Тип заявки: {requestType.Type}
                Дата начала: {request.DateTimeFrom}
                Дата конца : {request.DateTimeTo}
                Роль на проекте: {projectRoleType.Type}
                Обязанности: {request.ProjectRoleComment}
                Подписавшие: {appr}
                Должны подписать: {wait}";

            var accounting =
                request.UserSignatures.FirstOrDefault(us => us.Approved && us.User.RoleId == (int)UserRoles.Accounting);

            if (accounting == null) return;

            var mailAddress = new MailAddress(accounting.User.Email);
            _mail.SendEmail(mailAddress, subject, body);
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

            var body =
                $@"
                Ваша Заявка Отклонена!
                Кто: {request.User.SecondName} {request.User.FirstName}
                Статус заявки: {stateDetailType.Type}
                Причина заявки: {request.Reason}
                Тип заявки: {requestType.Type}
                Дата начала: {request.DateTimeFrom}
                Дата конца : {request.DateTimeTo}
                Роль на проекте: {projectRoleType.Type}
                Обязанности: {request.ProjectRoleComment}
                Подписавшие: {appr}
                Должны подписать: {wait}
                Отклонена: {reje}
                Причина отклонения: {reason}

                Вы можете учесть замечания и оставить новую заявку.";

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
                $"Заявка Отклонена {request.User.SecondName} {request.User.FirstName} [{request.Reason}] <{requestType.Type}> ({request.DateTimeFrom} - {request.DateTimeTo})";

            var body =
                $@"
                Заявка Отклонена!
                Кто: {request.User.SecondName} {request.User.FirstName}
                Статус заявки: {stateDetailType.Type}
                Причина заявки: {request.Reason}
                Тип заявки: {requestType.Type}
                Дата начала: {request.DateTimeFrom}
                Дата конца : {request.DateTimeTo}
                Роль на проекте: {projectRoleType.Type}
                Обязанности: {request.ProjectRoleComment}
                Подписавшие: {appr}
                Должны подписать: {wait}
                Отклонена: {reje}
                Причина отклонения: {reason}";

            var userSignatures = request.UserSignatures.Where(us => us.Approved).ToList();
            foreach (var mailAddress in userSignatures.Select(us => new MailAddress(us.User.Email)))
            {
                _mail.SendEmail(mailAddress, subject, body);
            }
        }
    }
}