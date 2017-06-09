namespace BL.Model.Enums
{
    /// <summary>
    /// Список операций над документов (основных и дополнительных)
    /// </summary>
    public enum EnumActions
    {
        Undefined = 0,

        #region Documens
        /// <summary>
        /// Создать документ по шаблону
        /// </summary>
        AddDocument = 100001,
        /// <summary>
        /// Создать связанный документ
        /// </summary>
        AddLinkedDocument = 100019,

        /// <summary>
        /// Создать документ копированием
        /// </summary>
        CopyDocument = 100002,

        /// <summary>
        /// Изменить документ
        /// </summary>
        ModifyDocument = 100003,

        /// <summary>
        /// Удалить документ
        /// </summary>
        DeleteDocument = 100004,

        /// <summary>
        /// Запустить выполнение плана
        /// </summary>
        LaunchPlan = 100005,

        /// <summary>
        /// Остановить выполнение плана
        /// </summary>
        StopPlan = 100007,

        /// <summary>
        /// Передать управление
        /// </summary>
        ChangeExecutor = 100008,

        /// <summary>
        /// Зарегистрировать документ
        /// </summary>
        RegisterDocument = 100009,

        /// <summary>
        /// Поменять позицию в документе
        /// </summary>
        ChangePosition = 100099,

        /// <summary>
        /// Направить для сведения
        /// </summary>
        SendForInformation = 100011,

        /// <summary>
        /// Направить для рассмотрения
        /// </summary>
        SendForConsideration = 100012,

        SendForInformationExternal = 100015, // Направить для сведения внешнему агенту

        SendDocument = 100018, // Направить документ

        /// <summary>
        /// Отметить прием
        /// </summary>
        //MarkReception = 100020, 

        /// <summary>
        /// Взять на контроль
        /// </summary>
        ControlOn = 100021,



        /// <summary>
        /// Изменить параметры контроля
        /// </summary>
        ControlChange = 100023,

        SendForExecutionChange = 100024, // Изменить параметры направлен для исполнения
        //SendForControlChange = 100025, // Изменить параметры направлен для исполнения (на контроль)
        //SendForResponsibleExecutionChange = 100026, // Изменить параметры направлен для исполнения (отв. исполнитель)

        /// <summary>
        /// Изменить параметры контроля для исполнителя
        /// </summary>
        ControlTargetChange = 100027,

        /// <summary>
        /// Снять с контроля
        /// </summary>
        ControlOff = 100029,

        /// <summary>
        /// Направить для контроля
        /// </summary>
        //SendForControl = 100030,

        /// <summary>
        /// Направить для ответственного исполнения 
        /// </summary>
        //SendForResponsibleExecution = 100031,

        /// <summary>
        /// Направить для исполнения
        /// </summary>
        SendForExecution = 100032,


        /// <summary>
        /// Попросить о переносе сроков исполнения
        /// </summary>
        AskPostponeDueDate = 100033,
        /// <summary>
        /// Отказать в переносе сроков исполнения
        /// </summary>
        CancelPostponeDueDate = 100034,
        /// <summary>
        /// Отметить исполнение
        /// </summary>
        MarkExecution = 100035,
        /// <summary>
        /// Отменить исполнение
        /// </summary>
        CancelExecution = 100036,
        /// <summary>
        /// Принять результат
        /// </summary>
        AcceptResult = 100037,

        /// <summary>
        /// Отклонить результат
        /// </summary>
        RejectResult = 100038,

        /// <summary>
        /// Направить для визирования 
        /// </summary>
        SendForVisaing = 100041,

        /// <summary>
        /// Направить для согласование 
        /// </summary>
        SendForАgreement = 100042,

        /// <summary>
        /// Направить для утверждения 
        /// </summary>
        SendForАpproval = 100043,

        /// <summary>
        /// Направить для подписи 
        /// </summary>
        SendForSigning = 100044,

        /// <summary>
        /// Отозвать с визирования
        /// </summary>
        WithdrawVisaing = 100046,

        /// <summary>
        /// Отозвать с согласования
        /// </summary>
        WithdrawАgreement = 100047,

        /// <summary>
        /// Отозвать с утверждения
        /// </summary>
        WithdrawАpproval = 100048,

        /// <summary>
        /// Отозвать с подписи
        /// </summary>
        WithdrawSigning = 100049,

        /// <summary>
        /// Завизировать
        /// </summary>
        AffixVisaing = 100051,

        /// <summary>
        /// Согласовать
        /// </summary>
        AffixАgreement = 100052,

        /// <summary>
        /// Утвердить
        /// </summary>
        AffixАpproval = 100053,

        /// <summary>
        /// Подписать
        /// </summary>
        AffixSigning = 100054,

        /// <summary>
        /// Самоподписание
        /// </summary>
        SelfAffixSigning = 100055,

        /// <summary>
        /// Отказать в визирования 
        /// </summary>
        RejectVisaing = 100056,

        /// <summary>
        /// Отказать в согласование
        /// </summary>
        RejectАgreement = 100057,

        /// <summary>
        /// Отказать в утверждения 
        /// </summary>
        RejectАpproval = 100058,

        /// <summary>
        /// Отказать в подписи 
        /// </summary>
        RejectSigning = 100059,

        VerifySigning = 100060,

        /// <summary>
        /// Направить сообщение участникам рабочей группы
        /// </summary>
        SendMessage = 100081,

        /// <summary>
        /// Добавить примечание
        /// </summary>
        AddNote = 100083,

        /// <summary>
        /// Добавить в избранное
        /// </summary>
        AddFavourite = 100091,

        /// <summary>
        /// Удалить из избранного
        /// </summary>
        DeleteFavourite = 100093,

        /// <summary>
        /// Закончить работу с документом
        /// </summary>
        FinishWork = 100095,

        /// <summary>
        ///  Возобновить работу с документом
        /// </summary>
        StartWork = 100097,

        /// <summary>
        /// Добавить ограничение рассылки
        /// </summary>
        AddDocumentRestrictedSendList = 102001,

        /// <summary>
        /// Добавить ограничения рассылки по стандартному списку
        /// </summary>
        AddByStandartSendListDocumentRestrictedSendList = 102002,

        /// <summary>
        /// Удалить ограничение рассылки
        /// </summary>
        DeleteDocumentRestrictedSendList = 102009,

        /// <summary>
        /// Добавить пункт плана
        /// </summary>
        AddDocumentSendList = 103001,
        /// <summary>
        /// Добавить пункт плана копированием
        /// </summary>
        CopyDocumentSendList = 103003,

        /// <summary>
        /// Добавить план работы по стандартному списку
        /// </summary>
        //AddByStandartSendListDocumentSendList = 103002,

        /// <summary>
        ///  Изменить документ
        /// </summary>
        ModifyDocumentSendList = 103005,

        /// <summary>
        /// Удалить пункт плана
        /// </summary>
        DeleteDocumentSendList = 103009,

        /// <summary>
        ///  Добавить этап плана
        /// </summary>
        AddDocumentSendListStage = 103011,

        /// <summary>
        /// Удалить этап плана
        /// </summary>
        DeleteDocumentSendListStage = 103019,
        /// <summary>
        /// Запустить пункт плана на исполнение
        /// </summary>
        LaunchDocumentSendListItem = 103021,
        /// <summary>
        /// Добавить файл
        /// </summary>
        AddDocumentFile = 104001,

        /// <summary>
        ///  Изменить файл
        /// </summary>
        ModifyDocumentFile = 104005,

        /// <summary>
        /// Удалить файл
        /// </summary>
        DeleteDocumentFile = 104009,
        /// <summary>
        /// Добавить версию файла к файлу
        /// </summary>
        AddDocumentFileUseMainNameFile = 104010,
        /// <summary>
        /// Файл принят
        /// </summary>
        AcceptDocumentFile = 104011,
        /// <summary>
        /// Файл не принят
        /// </summary>
        RejectDocumentFile = 104012,

        /// <summary>
        /// Переименовать файл
        /// </summary>
        RenameDocumentFile = 104013,

        /// <summary>
        /// Удалить версию файл
        /// </summary>
        DeleteDocumentFileVersion = 104014,
        /// <summary>
        /// Восстановить версию файла из корзину
        /// </summary>
        RestoreDocumentFileVersion = 104015,

        /// <summary>
        /// Сделать основной версией
        /// </summary>
        AcceptMainVersionDocumentFile = 104016,

        /// <summary>
        /// Удалить версию файл
        /// </summary>
        DeleteDocumentFileVersionFinal = 104018,
        /// <summary>
        /// Добавить связь между документами
        /// </summary>
        AddDocumentLink = 105001,

        /// <summary>
        /// Удалить связь между документами
        /// </summary>
        DeleteDocumentLink = 105009,

        /// <summary>
        /// Добавить сохраненный фильтр
        /// </summary>
        AddSavedFilter = 191001,
        /// <summary>
        /// Изменить сохраненный фильтр
        /// </summary>
        ModifySavedFilter = 191005,
        /// <summary>
        /// Удалить сохраненный фильтр
        /// </summary>
        DeleteSavedFilter = 191009,

        /// <summary>
        /// Изменить тэги по документу
        /// </summary>
        ModifyDocumentTags = 192005,

        /// <summary>
        ///  Отметить прочтение событий
        /// </summary>
        MarkDocumentEventAsRead = 100010,

        AddDocumentPaper = 121001, // Добавить бумажный носитель
//        CopyDocumentPaper = 121003, // Отметить создание копий бумажных носителей
        ModifyDocumentPaper = 121005, // Изменить бумажный носитель
        MarkOwnerDocumentPaper = 121007, // Отметить нахождение бумажного носителя у себя
        MarkСorruptionDocumentPaper = 121008, // Отметить порчу бумажного носителя
        DeleteDocumentPaper = 121009, // Удалить бумажный носитель
        PlanDocumentPaperEvent = 122001, // Планировать движение бумажного носителя
        CancelPlanDocumentPaperEvent = 122009, // Отменить планирование движения бумажного носителя
        SendDocumentPaperEvent = 122011, // Отметить передачу бумажного носителя
        CancelSendDocumentPaperEvent = 122019, // Отменить передачу бумажного носителя
        RecieveDocumentPaperEvent = 122021, // Отметить прием бумажного носителя

        //TODO Добавить в базу
        AddDocumentTask = 115001, // Добавить задачу
        ModifyDocumentTask = 115005, // Изменить задачу
        DeleteDocumentTask = 115009, // Удалить задачу
        AddDocumentPaperList = 123001, // Добавить реестр
        ModifyDocumentPaperList = 123005, // Изменить реестр
        DeleteDocumentPaperList = 123009, // Удалить реестр
        ReportRegistrationCardDocument = 100085,
        ReportRegisterTransmissionDocuments = 100086,
        ReportDocumentForDigitalSignature = 100087,
        VerifyPdf = 401006,
        #endregion Documents

        #region Templates
        AddTemplate = 251001,
        CopyTemplate = 251002,
        ModifyTemplate = 251005,
        DeleteTemplate = 251009,
        AddTemplateSendList = 252001,
        ModifyTemplateSendList = 252005,
        DeleteTemplateSendList = 252009,
        AddTemplateRestrictedSendList = 253001,
        ModifyTemplateRestrictedSendList = 253005,
        DeleteTemplateRestrictedSendList = 253009,
        AddTemplateTask = 254001,
        ModifyTemplateTask = 254005,
        DeleteTemplateTask = 254009,
        AddTemplateFile = 255001,
        ModifyTemplateFile = 255005,
        DeleteTemplateFile = 255009,

        AddTemplatePaper = 256001,
        ModifyTemplatePaper = 256005,
        DeleteTemplatePaper = 256009,

        AddTemplateAccess = 257001,
        ModifyTemplateAccess = 257005,
        DeleteTemplateAccess = 257009,
        #endregion Templates

        #region EncryptionCertificates
        /// <summary>
        /// Добавить сертификат
        /// </summary>
        AddEncryptionCertificate = 401001,
        /// Изменить сертификат
        /// </summary>
        ModifyEncryptionCertificate = 401002,
        /// <summary>
        /// Удалить сертификат
        /// </summary>
        DeleteEncryptionCertificate = 401004,
        #endregion EncryptionCertificates

        #region Dictionary
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


        AddBankContact = 229001,
        ModifyBankContact = 229005,
        DeleteBankContact = 229009,

        AddCompanyContact = 230001,
        ModifyCompanyContact = 230005,
        DeleteCompanyContact = 230009,

        AddEmployeeContact = 231001,
        ModifyEmployeeContact = 231005,
        DeleteEmployeeContact = 231009,

        AddClientCompanyContact = 232001,
        ModifyClientCompanyContact = 232005,
        DeleteClientCompanyContact = 232009,

        AddPersonContact = 233001,
        ModifyPersonContact = 233005,
        DeletePersonContact = 233009,




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

        AddBankAddress = 222001,
        ModifyBankAddress = 222005,
        DeleteBankAddress = 222009,

        AddCompanyAddress = 223001,
        ModifyCompanyAddress = 223005,
        DeleteCompanyAddress = 223009,

        AddEmployeeAddress = 224001,
        ModifyEmployeeAddress = 224005,
        DeleteEmployeeAddress = 224009,

        AddClientCompanyAddress = 225001,
        ModifyClientCompanyAddress = 225005,
        DeleteClientCompanyAddress = 225009,

        AddPersonAddress = 226001,
        ModifyPersonAddress = 226005,
        DeletePersonAddress = 226009,

        #endregion AgentAddress


        ModifyAgentPeoplePassport = 234005,

        // Persons
        #region AgentPersons
        /// <summary>
        /// добавить физлицо
        /// </summary>
        AddAgentPerson = 209001,

        AddAgentPersonExisting = 209002,
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
        ModifyAgentEmployee = 212005,
        /// <summary>
        /// удалить сотрудника
        /// </summary>
        DeleteAgentEmployee = 212009,
        /// <summary>
        /// изменить сотрудника
        /// </summary>
        ModifyAgentEmployeeLanguage = 212002,


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
        AddAgentCompany = 213001,
        /// <summary>
        /// изменить юрлицо
        /// </summary>
        ModifyAgentCompany = 213005,
        /// <summary>
        /// удалить юрлицо
        /// </summary>
        DeleteAgentCompany = 213009,
        /// <summary>
        /// добавить банк
        /// </summary>
        AddAgentBank = 214001,
        /// <summary>
        /// изменить банк
        /// </summary>
        ModifyAgentBank = 214005,
        /// <summary>
        /// удалить банк
        /// </summary>
        DeleteAgentBank = 214009,
        /// <summary>
        /// добавить расчетный счет
        /// </summary>
        AddAgentAccount = 215001,
        /// <summary>
        /// изменить расчетный счет
        /// </summary>
        ModifyAgentAccount = 215005,
        /// <summary>
        /// удалить расчетный счет
        /// </summary>
        DeleteAgentAccount = 215009,
        /// <summary>
        /// добавить содержание типового списка рассылки
        /// </summary>
        AddStandartSendListContent = 216001,
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
        AddOrg = 218001,

        /// <summary>
        /// Изменить запись в справочнике "Компании"
        /// </summary>
        ModifyOrg = 218005,

        /// <summary>
        /// Удалить запись в справочнике "Компании"
        /// </summary>
        DeleteOrg = 218009,
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

        #region  UserPositionExecutor
        AddUserPositionExecutor = 220006,

        ModifyUserPositionExecutor = 220007,

        DeleteUserPositionExecutor = 220008,
        #endregion 

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
        DeleteCustomDictionary = 302009,
        #endregion CustomDictionary
        #endregion Dictionary

        #region System
        SetSetting = 900001,
        Login = 1, //Вход в систему
        #endregion System

        #region Properties
        AddProperty = 311001, // Добавить динамический аттрибут
        ModifyProperty = 311005, // Изменить динамический аттрибут
        DeleteProperty = 311009, // Удалить динамический аттрибут
        AddPropertyLink = 312001, // Добавить связь динамических аттрибутов
        ModifyPropertyLink = 312005, // Изменить связь динамических аттрибутов
        DeletePropertyLink = 312009, // Удалить связь динамических аттрибутов

        ModifyPropertyValues = 313005,
        #endregion Properties

        #region Admin
        #region [+] Roles ...
        /// <summary>
        /// Добавить
        /// </summary>
        AddRole = 700001,
        /// <summary>
        /// Изменить
        /// </summary>
        ModifyRole = 700005,
        /// <summary>
        /// Удалить 
        /// </summary>
        DeleteRole = 700009,
        #endregion

        #region [+] RoleActions ...
        SetRolePermission = 701000,
        /// <summary>
        /// Добавить
        /// </summary>
        SetRolePermissionByModuleFeature = 701001,
        /// <summary>
        /// Изменить
        /// </summary>
        SetRolePermissionByModule = 701002,

        SetRolePermissionByModuleAccessType = 701003,
        #endregion

        #region [+] PositionRole ...
        SetPositionRole = 702000,
        /// <summary>
        /// Дублировать 
        /// </summary>
        DuplicatePositionRoles = 702002,
        #endregion

        #region [+] UserRoles ...
        SetUserRole = 703000,
        /// <summary>
        /// Добавить
        /// </summary>
        SetUserRoleByAssignment = 703001,
        /// <summary>
        /// Изменить
        /// </summary>
        //ModifyUserRole = 703005,
        /// <summary>
        /// Удалить 
        /// </summary>
        //DeleteUserRoleByUser = 703007,
        //DeleteUserRoleByPositionExecutor = 703008,
        // DeleteUserRole = 703009,
        #endregion

        #region [+] Subordinations ...
        ///// <summary>
        ///// Добавить
        ///// </summary>
        //AddSubordination = 704001,
        ///// <summary>
        ///// Изменить
        ///// </summary>
        //ModifySubordination = 704005,
        ///// <summary>
        ///// Удалить 
        ///// </summary>
        //DeleteSubordination = 704009,

        /// <summary>
        /// Совокупность действий по управлению субординацией
        /// </summary>
        SetSubordination = 704001,

        /// <summary>
        /// Копирование рассылки
        /// </summary>
        DuplicateSubordinations = 704002,
        /// <summary>
        /// Совокупность действий по управлению субординацией для всего подразделения
        /// </summary>
        SetSubordinationByDepartment = 704003,

        /// <summary>
        /// Совокупность действий по управлению субординацией для всей компании
        /// </summary>
        SetSubordinationByCompany = 704004,

        /// <summary>
        /// Совокупность действий по управлению субординацией для всей компании
        /// </summary>
        SetDefaultSubordination = 704005,

        /// <summary>
        /// Разрешить все
        /// </summary>
        SetAllSubordination = 704006,
        #endregion

        #region [+] RegistrationJournalPositions ...

        /// <summary>
        /// Совокупность действий по управлению доступом к журналам
        /// </summary>
        SetJournalAccess = 706000,

        /// <summary>
        /// Копирование доступа от журнала к журналу
        /// </summary>
        DuplicateJournalAccess_Journal = 706001,
        /// <summary>
        /// Копирование доступа от должнсости к должности
        /// </summary>
        DuplicateJournalAccess_Position = 706002,


        /// <summary>
        /// Управлению доступом к журналам
        /// </summary>
        SetJournalAccessByDepartment_Journal = 706003,

        /// <summary>
        /// Управлению доступом к журналам
        /// </summary>
        SetJournalAccessByDepartment_Position = 706004,

        /// <summary>
        /// Управлению доступом к журналам
        /// </summary>
        SetJournalAccessByCompany_Journal = 706005,
        /// <summary>
        /// Управлению доступом к журналам
        /// </summary>
        SetJournalAccessByCompany_Position = 706006,

        /// <summary>
        /// Управлению доступом к журналам
        /// </summary>
        SetJournalAccessDefault_Journal = 706007,

        /// <summary>
        /// Управлению доступом к журналам
        /// </summary>
        SetJournalAccessDefault_Position = 706008,


        /// <summary>
        /// Управлению доступом к журналам
        /// </summary>
        SetJournalAccessAll_Journal = 706009,

        /// <summary>
        /// Управлению доступом к журналам
        /// </summary>
        SetJournalAccessAll_Position = 706010,

        #endregion

        #region [+] DepartmentAdmin ...

        /// <summary>
        /// Добавить администратора подразделения
        /// </summary>
        AddDepartmentAdmin = 210003,

        /// <summary>
        /// Удалить администратора подразделения
        /// </summary>
        DeleteDepartmentAdmin = 210002,

        #endregion

        #region [+] Action over AgentUser
        //ChangePassword = 221001,
        //ChangeLockout = 221002,
        //KillSessions = 221003,
        ChangeLogin = 221004,
        //MustChangePassword = 221005,

        #endregion

        #endregion Admin
        // При добавлении действия не забудь добавить действие в ImportData: BL.Database.DatabaseContext.DmsDbImportData!!!

    }
}