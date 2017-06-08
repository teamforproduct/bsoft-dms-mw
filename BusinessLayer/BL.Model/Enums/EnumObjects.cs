namespace BL.Model.Enums
{
    /// <summary>
    /// Список обьектов системы
    /// </summary>
    public enum EnumObjects
    {
        System = 0, //Система
        Documents = 100, // Документы
        DocumentAccesses = 101, // Документы - доступы
        DocumentRestrictedSendLists = 102, // Документы - ограничения рассылки
        DocumentSendLists = 103, // Документы - план работы
        DocumentFiles = 104, // Документы - файлы
        DocumentLinks = 105, // Документы - связи
        DocumentSendListStages = 106, // Документы - этапы плана работ
        DocumentEvents = 111, // Документы - события
        DocumentWaits = 112, // Документы - ожидания
        DocumentSubscriptions = 113, // Документы - подписи
        DocumentTasks = 115, // Документы - задачи

        DocumentSendListAccessGroups = 117, // Документы - план группы получателей
        DocumentEventAccessGroups = 118, // Документы - события группы получателей
        DocumentEventAccesses = 119, // Документы - события получатели

        DocumentPapers = 121, // Документы - бумажные носители
        DocumentPaperEvents = 122, // Документы - события по бумажным носителям
        DocumentPaperLists = 123, // Документы - реестры передачи бумажных носителей

        DocumentSavedFilters = 191, // Документы - сохраненные фильтры
        DocumentTags = 192, // Документы - тэги
        DictionaryDocumentType = 201, // Типы документов
        DictionaryAddressType = 202, // Типы адресов
        DictionaryDocumentSubjects = 203, // Тематики документов
        DictionaryRegistrationJournals = 204, // Журналы регистрации
        DictionaryContactType = 205, // Типы контактов
        DictionaryAgents = 206, // Контрагенты
        DictionaryContacts = 207, // Контакты
        DictionaryAgentAddresses = 208, // Адреса
        DictionaryAgentPersons = 209, // Физические лица
        DictionaryDepartments = 210, // Структура предприятия
        DictionaryPositions = 211, // Штатное расписание
        DictionaryAgentEmployees = 212, // Сотрудники
        DictionaryAgentCompanies = 213, // Юридические лица
        DictionaryAgentBanks = 214, // Контрагенты - банки
        DictionaryAgentAccounts = 215, // Расчетные счета
        DictionaryStandartSendListContent = 216, // Типовые списки рассылки (содержание)
        DictionaryStandartSendLists = 217, // Типовые списки рассылки
        DictionaryAgentClientCompanies = 218, // Компании
        DictionaryPositionExecutorTypes = 219, // Типы исполнителей
        DictionaryPositionExecutors = 220, // Исполнители должности
        DictionaryAgentUsers = 221, // Контрагенты - пользователи
        DictionaryBankAddress = 222, // Контрагенты - пользователи
        DictionaryCompanyAddress = 223, // Контрагенты - пользователи
        DictionaryEmployeeAddress = 224, // Контрагенты - пользователи
        DictionaryClientCompanyAddress = 225, // Контрагенты - пользователи
        DictionaryPersonAddress = 226, // Контрагенты - пользователи


        DictionaryBankContact = 229,
        DictionaryCompanyContact = 230,
        DictionaryEmployeeContact = 231,
        DictionaryClientCompanyContact = 232,
        DictionaryPersonContact = 233,
        DictionaryAgentPeople = 234,


        Template = 251, // Шаблоны документов
        TemplateSendList = 252, // Списки рассылки в шаблонах
        TemplateRestrictedSendList = 253, // Ограничительные списки рассылки в шаблонах
        TemplateTask = 254, // Задачи в шаблонах
        TemplateFiles = 255, // Прикрепленные к шаблонам файлы
        TemplatePaper = 256, // БН в шаблонах
        TemplateAccess = 257, // Доступы к шаблонам
        DictionaryTag = 291, // Теги
        CustomDictionaryTypes = 301, // Типы пользовательских словарей
        CustomDictionaries = 302, // Пользовательские словари
        Properties = 311, // Динамические аттрибуты
        PropertyLinks = 312, // Связи динамических аттрибутов с объектами системы
        PropertyValues = 313, // Значения динамических аттрибутов

        EncryptionCertificates = 401, // Хранилище сертификатов
        //EncryptionCertificateTypes = 402,

        AdminRoles = 700,
        AdminRolePermission = 701,
        AdminPositionRoles = 702,
        AdminUserRoles = 703,
        AdminSubordination = 704,
        AdminRegistrationJournalPositions = 706,
        AdminEmployeeDepartments = 707,


        SystemSettings = 900,
        SystemObjects = -1,
        SystemActions = 1,
        

    }
}