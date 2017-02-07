namespace BL.Model.Enums
{
    public enum Roles
    {
        
        /// <summary>
        /// Администратор
        /// </summary>
        Admin = 100,

        /// <summary>
        /// Пользователь с правом выполнения действий с документом
        /// </summary>
        User = 200,

        /// <summary>
        /// Пользователь с правом просмотра
        /// </summary>
        Viewer = 210,

        /// <summary>
        /// Инструменты - история подключения
        /// </summary>
        Auditlog = 300,

        /// <summary>
        /// Инструменты - управление доступом к документам
        /// </summary>
        DocumAccess = 500,

        /// <summary>
        /// Документы - право выполнения действий
        /// </summary>
        DocumActions =510,

        /// <summary>
        /// Документы - право управления бумажными носителями
        /// </summary>
        DocumPapers = 520,

        /// <summary>
        /// Документы - право подписания
        /// </summary>
        DocumSign = 530,

        /// <summary>
        /// Документы - право контроля по документам
        /// </summary>
        DocumWaits = 550,




        /// <summary>
        /// Справочники - управление контрагентами
        /// </summary>
        ManagementAgents = 600,

        /// <summary>
        /// Справочники - управление авторизацией пользователей
        /// </summary>
        ManagementAuth = 610,

        /// <summary>
        /// Справочники - управление контактными лицами котрагентов
        /// </summary>
        ManagementContactPersons = 620,

        /// <summary>
        /// Справочники - управление справочниками СЭДО
        /// </summary>
        ManagementDocumDictionaries = 630,

        /// <summary>
        /// Справочники - управление сотрудниками
        /// </summary>
        ManagementEmployees = 640,

        /// <summary>
        /// Справочники - управление журналами организации
        /// </summary>
        ManagementJournals = 650,

        /// <summary>
        /// Администратор - управление структурой организации
        /// </summary>
        ManagementOrg = 670,

    }
}