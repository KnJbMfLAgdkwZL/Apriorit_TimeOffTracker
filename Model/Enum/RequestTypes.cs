namespace TimeOffTracker.Model.Enum
{
    public enum RequestTypes
    {
        /// <summary>
        /// Очередной оплачиваемый отпуск
        /// </summary>
        PaidLeave = 1,

        /// <summary>
        /// Административный (неоплачиваемый) отпуск
        /// </summary>
        AdministrativeUnpaidLeave = 2,

        /// <summary>
        /// Административный отпуск по причине форс-мажора
        /// </summary>
        ForceMajeureAdministrativeLeave = 3,

        /// <summary>
        /// Учебный отпуск
        /// </summary>
        StudyLeave = 4,

        /// <summary>
        /// Социальный отпуск (по причине смерти близкого)
        /// </summary>
        SocialLeave = 5,

        /// <summary>
        /// Больничный с больничным листом
        /// </summary>
        SickLeaveWithDocuments = 6,

        /// <summary>
        /// Больничный без больничного листа
        /// </summary>
        SickLeaveWithoutDocuments = 7
    }
}