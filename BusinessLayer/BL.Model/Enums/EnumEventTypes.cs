namespace BL.Model.Enums
{
    public enum EnumEventTypes
    {
        AddNewDocument = 110, //Создан проект
        ControlOn = 300,//Взят на контроль
        ControlChange = 310,//Изменить контроль
        ControlTargetChange = 315,//Изменить параметры контроля для исполнителя
        ControlOff = 301,//Снят с контроля

        SendForExecution = 210,//   Направлен для исполнения
        SendForExecutionChange = 211,//   Изменить параметры направлен для исполнения
        SendForControl = 212,//    Направлен для исполнения (на контроль)
//        SendForControlChange = 213,//    Изменить параметры направлен для исполнения (на контроль)
        SendForResponsibleExecution = 214,//  Направлен для исполнения (отв. исполнитель)
        SendForResponsibleExecutionChange = 215,//   Изменить параметры направлен для исполнения (отв. исполнитель)

        TaskFormulation = 601,//Формулировка задачи

        MarkExecution = 320,//Поручение выполнено
        AcceptResult = 321,//Результат принят
        RejectResult = 322,//Результат отклонен

        AddNote = 600,//Примечание
        SetInWork = 998,//	Работа возобновлена
        SetOutWork = 999,//	Работа завершена
        SendMessage = 500,//	Направлено сообщение
        ChangeExecutor = 205,// Изменение исполнителя по документу
        ChangePosition = 207,// Замена должности в документе
        SendForInformation = 200,//	Направлен для сведения

        SendForConsideration = 220,//	Направлен для рассмотрения
        SendForInformationExternal = 230, //Направлен для сведения внешнему агенту

        SendForVisaing = 250,// Направить для визирования 
        SendForАgreement = 260,// Направить для согласование 
        SendForАpproval = 270,// Направить для утверждения 
        SendForSigning = 280,// Направить для подписи 



        AffixVisaing = 251,//	Завизирован
        RejectVisaing = 252,//	Отказано в визировании
        WithdrawVisaing = 253,//	Отозван с визирования

        AffixАgreement = 261,//	Согласован
        RejectАgreement = 262,//	Отказано в согласовании
        WithdrawАgreement = 263,//	Отозван с согласования

        AffixАpproval = 271,//	Утвержден
        RejectАpproval = 272,//	Отказано в утверждении
        WithdrawАpproval = 273,//	Отозван с утверждения

        AffixSigning = 281,//	Подписан
        RejectSigning = 282,//	Отказано в подписании
        WithdrawSigning = 283,//	Отозван с подписания



        Registered = 701,// Зарегистрирован

        LaunchPlan = 800, //   Запущено исполнение плана работы по документу
        StopPlan = 810,// Остановлено исполнение плана работы по документу

        AddNewPaper = 505, //Добавлен бумажный носитель
        MarkOwnerDocumentPaper = 507, //Отметка нахождения бумажного носителя у себя
        MarkСorruptionDocumentPaper = 509, //Отметка порчи бумажного носителя

        MoveDocumentPaper = 510, //Переданы бумажные носители

        AddLink = 550, //Добавлена связь
        DeleteLink = 555, //Удалена связь

        AddDocumentFile = 115, //    Добавлен файл
        RanameDocumentFile = 116, //    Переименован файл
        ModifyDocumentFile = 117, //    Изменен файл
        DeleteDocumentFileVersion = 118,
        DeleteDocumentFile = 119, //    Удален файл
        RejectDocumentFile = 121, //
        AcceptDocumentFile = 122, //
    }
}