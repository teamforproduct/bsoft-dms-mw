namespace BL.Model.Enums
{
    /// <summary>
    /// Список экшенов по словарям
    /// </summary>
    public enum EnumDictionaryActions
    {
        // Типы документов
        #region DictionaryDocumentType
        /// <summary>
        /// Добавить тип документа
        /// </summary>
        AddDocumentType = 201001,
        /// <summary>
        /// Изменить тип документа
        /// </summary>
        ModifyDocumentType = 201005,
        /// <summary>
        /// Удалить тип документа
        /// </summary>
        DeleteDocumentType = 201009,
        #endregion DictionaryAddressType

        // Типы адресов
        #region DictionaryAddressType
        /// <summary>
        /// Добавить тип адреса
        /// </summary>
        AddAddressType = 202001,
        /// <summary>
        /// Изменить тип адреса
        /// </summary>       
        ModifyAddressType = 202005,
        /// <summary>
        /// Удалить тип адреса
        /// </summary>       
        DeleteAddressType = 202009,
        #endregion DictionaryAddressType

        // Тематики документов
        #region DictionaryDocumentSubject
        /// <summary>
        /// Добавить запись в справочнике "Тематики документов"
        /// </summary>
        AddDocumentSubject = 203001,

        /// <summary>
        /// Изменить запись в справочнике "Тематики документов"
        /// </summary>       
        ModifyDocumentSubject = 203005,

        /// <summary>
        /// Удалить запись в справочнике "Тематики документов"
        /// </summary>       
        DeleteDocumentSubject = 203009,
        #endregion DictionaryDocumentSubject

        // Журналы регистрации
        #region DictionaryRegistrationJournal
        /// <summary>
        /// Добавить запись в справочнике "Журналы регистрации"
        /// </summary>
        AddRegistrationJournal = 204001,

        /// <summary>
        /// Изменить запись в справочнике "Журналы регистрации"
        /// </summary>
        ModifyRegistrationJournal = 204005,

        /// <summary>
        /// Удалить запись в справочнике "Журналы регистрации"
        /// </summary>
        DeleteRegistrationJournal = 204009,
        #endregion DictionaryRegistrationJournal

        // Типы контактов
        #region DictionaryContactType
        AddContactType = 205001,
        ModifyContactType = 205005,
        DeleteContactType = 205009,
        #endregion ContactType

        // Агенты
        #region Agent
        /// <summary>
        /// добавить контраента
        /// </summary>
        AddAgent = 206001,
        /// <summary>
        /// изменить контрагента
        /// </summary>
        ModifyAgent = 206005,
        /// <summary>
        /// удалить контрагента
        /// </summary>
        DeleteAgent = 206009,
        #endregion Agent

        // Контакты
        #region Contacts
        /// <summary>
        /// Добавить контакт
        /// </summary>
        AddContact = 207001,
        /// <summary>
        /// Изменить контакт
        /// </summary>
        ModifyContact = 207005,
        /// <summary>
        /// Удалить контакт
        /// </summary>
        DeleteContact = 207009,
        #endregion Contacts

        // Адреса
        #region AgentAddress
        /// <summary>
        /// добавить адрес
        /// </summary>
        AddAgentAddress = 208001,
        /// <summary>
        /// изменить адрес
        /// </summary>
        ModifyAgentAddress = 208005,
        /// <summary>
        /// удалить адрес
        /// </summary>
        DeleteAgentAddress = 208009,
        #endregion AgentAddress

        // Persons
        #region AgentPersons
        /// <summary>
        /// добавить физлицо
        /// </summary>
        AddAgentPerson = 209001,
        /// <summary>
        /// изменить физлицо
        /// </summary>
        ModifyAgentPerson = 209005,
        /// <summary>
        /// удалить физлицо
        /// </summary>
        DeleteAgentPerson = 209009,
        #endregion AgentPersons

        // Структура предприятия
        #region DictionaryDepartment
        /// <summary>
        /// Добавить запись в справочнике "Структура предприятия"
        /// </summary>
        AddDepartment = 210001,

        /// <summary>
        /// Изменить запись в справочнике "Структура предприятия"
        /// </summary>
        ModifyDepartment = 210005,

        /// Удалить запись в справочнике "Структура предприятия"
        /// </summary>
        DeleteDepartment = 210009,
        #endregion DictionaryDepartment
     
        // Штатное расписание
        #region DictionaryPositions
        /// <summary>
        /// Добавить запись в справочнике "Штатное расписание"
        /// </summary>
        AddPosition = 211001,

        /// <summary>
        /// Изменить запись в справочнике "Штатное расписание"
        /// </summary>
        ModifyPosition = 211005,

        /// <summary>
        /// Удалить запись в справочнике "Штатное расписание"
        /// </summary>
        DeletePosition = 211009,
        #endregion DictionaryDepartment

        // Сотрудники
        #region AgentEmployee
        /// <summary>
        /// добавить сотрудника
        /// </summary>
        AddAgentEmployee = 212001,
        /// <summary>
        /// изменить сотрудника
        /// </summary>
        ModifyAgentEmployee=212005,
        /// <summary>
        /// удалить сотрудника
        /// </summary>
        DeleteAgentEmployee=212009,
        #endregion AgentEmployee
        
        // Тэги
        #region Tags
        /// <summary>
        /// Добавить тэг
        /// </summary>
        AddTag = 291001,
        /// <summary>
        /// Изменить тэг
        /// </summary>
        ModifyTag = 291005,
        /// <summary>
        /// Удалить тэг
        /// </summary>
        DeleteTag = 291009,
        #endregion Tags

        /// <summary>
        /// добавить юрлицо
        /// </summary>
        AddAgentCompany=213001,
        /// <summary>
        /// изменить юрлицо
        /// </summary>
        ModifyAgentCompany=213005,
        /// <summary>
        /// удалить юрлицо
        /// </summary>
        DeleteAgentCompany =213009,
        /// <summary>
        /// добавить банк
        /// </summary>
        AddAgentBank,
        /// <summary>
        /// изменить банк
        /// </summary>
        ModifyAgentBank,
        /// <summary>
        /// удалить банк
        /// </summary>
        DeleteAgentBank,

        #region CustomDictionaryType
        /// <summary>
        /// Добавить тип пользовательского словаря
        /// </summary>
        AddCustomDictionaryType,
        /// <summary>
        /// Редактировать тип пользовательского словаря
        /// </summary>
        ModifyCustomDictionaryType,
        /// <summary>
        /// Удалить тип пользовательского словаря
        /// </summary>
        DeleteCustomDictionaryType,
        #endregion CustomDictionaryType

        #region CustomDictionary
        /// <summary>
        /// Добавит пользовательсткий словать
        /// </summary>
        AddCustomDictionary,
        /// <summary>
        /// Редактировать пользовательский словарь
        /// </summary>
        ModifyCustomDictionary,
        /// <summary>
        /// Удалить пользовательский словарь
        /// </summary>
        DeleteCustomDictionary
        #endregion CustomDictionary
    }
}