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
        #endregion DictionaryDocumentType

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
        /// <summary>
        /// Добавить тип контакта
        /// </summary>
        AddContactType = 205001,
        /// <summary>
        /// Изменить тип контакта
        /// </summary>
        ModifyContactType = 205005,
        /// <summary>
        /// Удалить тип контакта
        /// </summary>
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
        /// <summary>
        /// добавить фото
        /// </summary>
        SetAgentImage = 206002,
        /// <summary>
        /// добавить фото
        /// </summary>
        DeleteAgentImage = 206003,
        #endregion Agent

        // Контакты
        #region Contacts
        /// <summary>
        /// Добавить контакт
        /// </summary>
        AddAgentContact = 207001,
        /// <summary>
        /// Изменить контакт
        /// </summary>
        ModifyAgentContact = 207005,
        /// <summary>
        /// Удалить контакт
        /// </summary>
        DeleteAgentContact = 207009,
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

        AddExecutorType = 219001,
        ModifyExecutorType = 219005,
        DeleteExecutorType = 219009,
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
        AddAgentCompany =213001,
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
        AddAgentBank=214001,
        /// <summary>
        /// изменить банк
        /// </summary>
        ModifyAgentBank=214005,
        /// <summary>
        /// удалить банк
        /// </summary>
        DeleteAgentBank=214009,
        /// <summary>
        /// добавить расчетный счет
        /// </summary>
        AddAgentAccount=215001,
        /// <summary>
        /// изменить расчетный счет
        /// </summary>
        ModifyAgentAccount=215005,
        /// <summary>
        /// удалить расчетный счет
        /// </summary>
        DeleteAgentAccount=215009,
        /// <summary>
        /// добавить содержание типового списка рассылки
        /// </summary>
        AddStandartSendListContent=216001,
        /// <summary>
        /// изменить  содержание типового списка рассылки
        /// </summary>
        ModifyStandartSendListContent = 216005,
        /// <summary>
        ///  удалить содержание типового списка рассылки
        /// </summary>
        DeleteStandartSendListContent = 216009,
        /// <summary>
        /// добавить типовой список рассылки
        /// </summary>
        AddStandartSendList = 217001,
        /// <summary>
        /// изменить типовой список рассылки
        /// </summary>
        ModifyStandartSendList = 217005,
        /// <summary>
        /// удалить типовой список рассылки
        /// </summary>
        DeleteStandartSendList = 217009,

        // Компании
        #region DictionaryCompanies
        /// <summary>
        /// Добавить запись в справочнике "Компании"
        /// </summary>
        AddAgentClientCompany = 218001,

        /// <summary>
        /// Изменить запись в справочнике "Компании"
        /// </summary>
        ModifyAgentClientCompany = 218005,

        /// <summary>
        /// Удалить запись в справочнике "Компании"
        /// </summary>
        DeleteAgentClientCompany = 218009,
        #endregion DictionaryCompanies




        // Компании
        #region DictionaryPositionExecutors
        /// <summary>
        /// Добавить запись в справочнике "Компании"
        /// </summary>
        AddExecutor = 220001,

        /// <summary>
        /// Изменить запись в справочнике "Компании"
        /// </summary>
        ModifyExecutor = 220005,

        /// <summary>
        /// Удалить запись в справочнике "Компании"
        /// </summary>
        DeleteExecutor = 220009,
        #endregion DictionaryPositionExecutors


        #region CustomDictionaryType
        /// <summary>
        /// Добавить тип пользовательского словаря
        /// </summary>
        AddCustomDictionaryType = 301001,
        /// <summary>
        /// Редактировать тип пользовательского словаря
        /// </summary>
        ModifyCustomDictionaryType = 301005,
        /// <summary>
        /// Удалить тип пользовательского словаря
        /// </summary>
        DeleteCustomDictionaryType = 301009,
        #endregion CustomDictionaryType

        #region CustomDictionary
        /// <summary>
        /// Добавит пользовательсткий словать
        /// </summary>
        AddCustomDictionary = 302001,
        /// <summary>
        /// Редактировать пользовательский словарь
        /// </summary>
        ModifyCustomDictionary = 302005,
        /// <summary>
        /// Удалить пользовательский словарь
        /// </summary>
        DeleteCustomDictionary = 302009
        #endregion CustomDictionary

        // При добавлении действия не забудь добавить действие в ImportData: BL.Database.DatabaseContext.DmsDbImportData!!!
    }
}