namespace BL.Model.FullTextSearch
{
    /// <summary>
    /// Типы фильтров для обновления фултекста
    /// </summary>
    public enum EnamFilterType
    {

        /// <summary>
        /// Основной
        /// </summary>
        Main = -1,
        /// <summary>
        /// Дочерние записи
        /// </summary>
        Slave = -2,
        /// <summary>
        /// Зависимые записи
        /// </summary>
        Dependant = -3,

        TemplateDocumentDocumentTypeId = 103,
        ExecutorPositionExecutorAgentId = 105,
        RegistrationJournalId = 110,
        RegistrationJournalDepartmentId = 110,
        SenderAgentId = 120,
        SenderAgentPersonId = 125,
        SourcePositionExecutorAgentId = 210,
        SourceAgentId = 215,
        TargetPositionExecutorAgentId = 220,
        TargetAgentId = 225,

        TagId = 410,

        DepartmentId = 510,




    }
}