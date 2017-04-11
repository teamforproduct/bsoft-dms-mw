namespace BL.Model.Enums
{
    /// <summary>
    /// Список операций над документов (основных и дополнительных)
    /// </summary>
    public enum EnumDocumentActions
    {
        Undefined = 0,
        /// <summary>
        /// Просмотр документов
        /// </summary>
        ViewDocument = 100000,
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
        /// Добавить пункт плана
        /// </summary>
        AddDocumentSendListItem = 100006,

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
        SendForResponsibleExecutionChange = 100026, // Изменить параметры направлен для исполнения (отв. исполнитель)

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
        SendForControl = 100030,

        /// <summary>
        /// Направить для ответственного исполнения 
        /// </summary>
        SendForResponsibleExecution = 100031,

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
        /// Удалить запись о версим файла
        /// </summary>
        DeleteDocumentFileVersionRecord = 104015,

        /// <summary>
        /// Сделать основной версией
        /// </summary>
        AcceptMainVersionDocumentFile = 104016,


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

        AddTemplateDocument = 251001,
        CopyTemplateDocument = 251002,
        ModifyTemplateDocument = 251005,
        DeleteTemplateDocument = 251009,
        AddTemplateDocumentSendList = 252001,
        ModifyTemplateDocumentSendList = 252005,
        DeleteTemplateDocumentSendList = 252009,
        AddTemplateDocumentRestrictedSendList = 253001,
        ModifyTemplateDocumentRestrictedSendList = 253005,
        DeleteTemplateDocumentRestrictedSendList = 253009,
        AddTemplateDocumentTask = 254001,
        ModifyTemplateDocumentTask = 254005,
        DeleteTemplateDocumentTask = 254009,
        AddTemplateAttachedFile = 255001,
        ModifyTemplateAttachedFile = 255005,
        DeleteTemplateAttachedFile = 255009,

        AddTemplateDocumentPaper = 256001,
        ModifyTemplateDocumentPaper = 256005,
        DeleteTemplateDocumentPaper = 256009,

        AddTemplateDocumentAccess = 257001,
        ModifyTemplateDocumentAccess = 257005,
        DeleteTemplateDocumentAccess = 257009,

        // При добавлении действия не забудь добавить действие в ImportData: BL.Database.DatabaseContext.DmsDbImportData!!!

    }
}