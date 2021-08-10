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
                <b>Запрос на отпуск!</b> <br/>

                <b>Кто:</b> {request.User.SecondName} {request.User.FirstName} <br/>
                <b>Статус заявки:</b> {stateDetailType.Type} <br/>
                <b>Причина заявки:</b> {request.Reason} <br/>
                <b>Тип заявки:</b> {requestType.Type} <br/>
                <b>Дата начала:</b> {request.DateTimeFrom} <br/>
                <b>Дата конца :</b> {request.DateTimeTo} <br/>
                <b>Роль на проекте:</b> {projectRoleType.Type} <br/>
                <b>Обязанности:</b> {request.ProjectRoleComment} <br/>
                <b>Подписавшие:</b> {appr} <br/>
                <b>Должны подписать:</b> {wait} <br/>

                <a href='http://localhost:5000/approveRequest?id={request.Id}'>Approve</a> 
                | 
                <a href='http://localhost:5000/rejectRequest?id={request.Id}'>Reject</a> 
            ";

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
                <b>Ваша Заявка Утверждена!</b> <br/>

                <b>Кто:</b> {request.User.SecondName} {request.User.FirstName} <br/>
                <b>Статус заявки:</b> {stateDetailType.Type} <br/>
                <b>Причина заявки:</b> {request.Reason} <br/>
                <b>Тип заявки:</b> {requestType.Type} <br/>
                <b>Дата начала:</b> {request.DateTimeFrom} <br/>
                <b>Дата конца :</b> {request.DateTimeTo} <br/>
                <b>Роль на проекте:</b> {projectRoleType.Type} <br/>
                <b>Обязанности:</b> {request.ProjectRoleComment} <br/>
                <b>Подписавшие:</b> {appr} <br/>
                <b>Должны подписать:</b> {wait} <br/>
            ";

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
                <b>Заявка Утверждена!</b> <br/>

                <b>Кто:</b> {request.User.SecondName} {request.User.FirstName} <br/>
                <b>Статус заявки:</b> {stateDetailType.Type} <br/>
                <b>Причина заявки:</b> {request.Reason} <br/>
                <b>Тип заявки:</b> {requestType.Type} <br/>
                <b>Дата начала:</b> {request.DateTimeFrom} <br/>
                <b>Дата конца :</b> {request.DateTimeTo} <br/>
                <b>Роль на проекте:</b> {projectRoleType.Type} <br/>
                <b>Обязанности:</b> {request.ProjectRoleComment} <br/>
                <b>Подписавшие:</b> {appr} <br/>
                <b>Должны подписать:</b> {wait} <br/>
            ";

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

            var rejectedUs = new List<UserSignature>();
            foreach (var us in request.UserSignatures)
            {
                if (us.Reason != null)
                {
                    if (us.Reason.Length > 0)
                    {
                        rejectedUs.Add(us);
                    }
                }
            }

            var rejected = rejectedUs.Select(us => us.User.SecondName + " " + us.User.FirstName).ToList();

            var appr = string.Join(", ", approved);
            var wait = string.Join(", ", waiting);
            var reje = string.Join(", ", rejected);
            var reason = rejectedUs.FirstOrDefault()?.Reason;

            var subject =
                $"Ваша Заявка Отклонена [{request.Reason}] <{requestType.Type}> ({request.DateTimeFrom} - {request.DateTimeTo})";

            var body =
                $@"
                <b>Ваша Заявка Отклонена! <br/>

                <b>Кто:</b> {request.User.SecondName} {request.User.FirstName} <br/>
                <b>Статус заявки:</b> {stateDetailType.Type} <br/>
                <b>Причина заявки:</b> {request.Reason} <br/>
                <b>Тип заявки:</b> {requestType.Type} <br/>
                <b>Дата начала:</b> {request.DateTimeFrom} <br/>
                <b>Дата конца :</b> {request.DateTimeTo} <br/>
                <b>Роль на проекте:</b> {projectRoleType.Type} <br/>
                <b>Обязанности:</b> {request.ProjectRoleComment} <br/>
                <b>Подписавшие:</b> {appr} <br/>
                <b>Должны подписать:</b> {wait} <br/>
                
                <b>Отклонена:</b> {reje} <br/>
                <b>Причина отклонения:</b> {reason} <br/>

                Вы можете учесть замечания и оставить новую заявку.
            ";

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

            var rejectedUs = new List<UserSignature>();
            foreach (var us in request.UserSignatures)
            {
                if (us.Reason != null)
                {
                    if (us.Reason.Length > 0)
                    {
                        rejectedUs.Add(us);
                    }
                }
            }

            var rejected = rejectedUs.Select(us => us.User.SecondName + " " + us.User.FirstName).ToList();

            var appr = string.Join(", ", approved);
            var wait = string.Join(", ", waiting);
            var reje = string.Join(", ", rejected);
            var reason = rejectedUs.FirstOrDefault()?.Reason;

            var subject =
                $"Заявка Отклонена {request.User.SecondName} {request.User.FirstName} [{request.Reason}] <{requestType.Type}> ({request.DateTimeFrom} - {request.DateTimeTo})";

            var body =
                $@"
                <b>Заявка Отклонена! <br/>

                <b>Кто:</b> {request.User.SecondName} {request.User.FirstName} <br/>
                <b>Статус заявки:</b> {stateDetailType.Type} <br/>
                <b>Причина заявки:</b> {request.Reason} <br/>
                <b>Тип заявки:</b> {requestType.Type} <br/>
                <b>Дата начала:</b> {request.DateTimeFrom} <br/>
                <b>Дата конца :</b> {request.DateTimeTo} <br/>
                <b>Роль на проекте:</b> {projectRoleType.Type} <br/>
                <b>Обязанности:</b> {request.ProjectRoleComment} <br/>
                <b>Подписавшие:</b> {appr} <br/>
                <b>Должны подписать:</b> {wait} <br/>

                <b>Отклонена:</b> {reje} <br/>
                <b>Причина отклонения:</b> {reason} <br/>
            ";

            var userSignatures = request.UserSignatures.Where(us => us.Approved).ToList();
            foreach (var mailAddress in userSignatures.Select(us => new MailAddress(us.User.Email)))
            {
                _mail.SendEmail(mailAddress, subject, body);
            }
        }
    }
}