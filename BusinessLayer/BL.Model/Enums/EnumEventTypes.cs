namespace BL.Model.Enums
{
    public enum EnumEventTypes
    {
        // Создан проект
        AddNewDocument = 110,
        // Взят на контроль
        ControlOn = 300,
        // Изменить контроль
        ControlChange = 310,
        // Изменить параметры контроля для исполнителя
        ControlTargetChange = 315,
        // Снят с контроля
        ControlOff = 301,


        // Направлен для исполнения
        SendForExecution = 210,
        // Изменить параметры направлен для исполнения
        SendForExecutionChange = 211,
        // Направлен для исполнения (на контроль)
        SendForControl = 212,
        // Изменить параметры направлен для исполнения (на контроль)
        // SendForControlChange = 213,
        // Направлен для исполнения (отв. исполнитель)
        SendForResponsibleExecution = 214,
        // Изменить параметры направлен для исполнения (отв. исполнитель)
        SendForResponsibleExecutionChange = 215,


        // Формулировка задачи
        TaskFormulation = 601,


        // Поручение выполнено
        MarkExecution = 320,
        // Результат принят
        AcceptResult = 321,
        // Результат отклонен
        RejectResult = 322,
        // Поручение отменено
        CancelExecution = 323,
        // Примечание
        AddNote = 600,
        // Работа возобновлена
        SetInWork = 998,
        // Работа завершена
        SetOutWork = 999,
        // Направлено сообщение
        SendMessage = 500,
        // Изменение исполнителя по документу
        ChangeExecutor = 205,
        // Замена должности в документе
        ChangePosition = 207,
        // Направлен для сведения
        SendForInformation = 200,


        // Направлен для рассмотрения
        SendForConsideration = 220,
        // Направлен для сведения внешнему агенту
        SendForInformationExternal = 230,


        // Направить для визирования
        SendForVisaing = 250,
        // Направить для согласование
        SendForАgreement = 260,
        // Направить для утверждения
        SendForАpproval = 270,
        // Направить для подписи
        SendForSigning = 280,






        // Завизирован
        AffixVisaing = 251,
        // Отказано в визировании
        RejectVisaing = 252,
        // Отозван с визирования
        WithdrawVisaing = 253,


        // Согласован
        AffixАgreement = 261,
        // Отказано в согласовании
        RejectАgreement = 262,
        // Отозван с согласования
        WithdrawАgreement = 263,


        // Утвержден
        AffixАpproval = 271,
        // Отказано в утверждении
        RejectАpproval = 272,
        // Отозван с утверждения
        WithdrawАpproval = 273,


        // Подписан
        AffixSigning = 281,
        // Отказано в подписании
        RejectSigning = 282,
        // Отозван с подписания
        WithdrawSigning = 283,


        // Зарегистрирован
        Registered = 701,


        // Запущено исполнение плана работы по документу
        LaunchPlan = 800,
        // Остановлено исполнение плана работы по документу
        StopPlan = 810,


        // Добавлен бумажный носитель
        AddNewPaper = 505,
        // Отметка нахождения бумажного носителя у себя
        MarkOwnerDocumentPaper = 507,
        // Отметка порчи бумажного носителя
        MarkСorruptionDocumentPaper = 509,


        // Переданы бумажные носители
        MoveDocumentPaper = 510,


        // Добавлена связь
        AddLink = 550,
        // Удалена связь
        DeleteLink = 555,


        // Добавлен файл
        AddDocumentFile = 115,
        // Переименован файл
        RanameDocumentFile = 116,
        // Изменен файл
        ModifyDocumentFile = 117,

        DeleteDocumentFileVersion = 118,
        // Удален файл
        DeleteDocumentFile = 119,
        // 
        RejectDocumentFile = 121,
        // 
        AcceptDocumentFile = 122,

    }
}