namespace BL.Model.Enums
{
    public enum EnumDocumentActions
    {
        AddDocument = 100001, // Создать документ по шаблону
        CopyDocument = 100002, // Создать документ копированием
        ModifyDocument = 100003, // Изменить документ
        DeleteDocument = 100004, // Удалить документ
        LaunchPlan = 100005, // Запустить выполнение плана
        StopPlan = 100006, // Остановить выполнение плана
        ChangeExecutor = 100008, // Передать управление
        RegisterDocument = 100009, // Зарегистрировать документ
        SendForInformation = 100011, // Направить для сведения
        SendForConsideration = 100012, // Направить для рассмотрения
        MarkReception = 100020, // Отметить прием
        ControlOn = 100021, // Взять на контроль
        SendForControl = 100022, // Направить для контроля
        ControlChange = 100025, // Изменить параметры контроля
        ControlOff = 100027, // Снять с контроля
        SendForResponsibleExecution = 100031, // Направить для ответственного исполнения 
        SendForExecution = 100032, // Направить для исполнения
        MarkExecution = 100035, // Отметить исполнение
        AcceptResult = 100037, // Принять результат
        RejectResult = 100038, // Отклонить результат
        SendForVisaing = 100041, // Направить для визирования 
        SendForАgreement = 100042, // Направить для согласование 
        SendForАpproval = 100043, // Направить для утверждения 
        SendForSigning = 100044, // Направить для подписи 
        WithdrawVisaing = 100046, // Отозвать с визирования
        WithdrawАgreement = 100047, // Отозвать с согласования
        WithdrawАpproval = 100048, // Отозвать с утверждения
        WithdrawSigning = 100049, // Отозвать с подписи
        AffixVisaing = 100051, // Завизировать
        AffixАgreement = 100052, // Согласовать
        AffixАpproval = 100053, // Утвердить 
        AffixSigning = 100054, // Подписать 
        RejectVisaing = 100056, // Отказать в визирования 
        RejectАgreement = 100057, // Отказать в согласование 
        RejectАpproval = 100058, // Отказать в утверждения 
        RejectSigning = 100059, // Отказать в подписи 
        SendMessage = 100081, // Направить сообщение участникам рабочей группы
        AddNote = 100083, // Добавить примечание
        AddFavourite = 100091, // Добавить в избранное
        DeleteFavourite = 100093, // Удалить из избранного
        FinishWork = 100095, // Закончить работу с документом
        StartWork = 100097, // Возобновить работу с документом
    }
}