﻿namespace BL.Model.Enums
{
    /// <summary>
    /// Список операций над документов (основных и дополнительных)
    /// </summary>
    public enum EnumDocumentActions
    {
        /// <summary>
        /// Создать документ по шаблону
        /// </summary>
        AddDocument = 100001, 

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
        StopPlan = 100006, 

        /// <summary>
        /// Передать управление
        /// </summary>
        ChangeExecutor = 100008, 

        /// <summary>
        /// Зарегистрировать документ
        /// </summary>
        RegisterDocument = 100009, 

        /// <summary>
        /// Направить для сведения
        /// </summary>
        SendForInformation = 100011, 

        /// <summary>
        /// Направить для рассмотрения
        /// </summary>
        SendForConsideration = 100012, 

        /// <summary>
        /// Отметить прием
        /// </summary>
        MarkReception = 100020, 

        /// <summary>
        /// Взять на контроль
        /// </summary>
        ControlOn = 100021, 

        /// <summary>
        /// Направить для контроля
        /// </summary>
        SendForControl = 100022, 

        /// <summary>
        /// Изменить параметры контроля
        /// </summary>
        ControlChange = 100025, 

        /// <summary>
        /// Снять с контроля
        /// </summary>
        ControlOff = 100027, 

        /// <summary>
        /// Направить для ответственного исполнения 
        /// </summary>
        SendForResponsibleExecution = 100031, 

        /// <summary>
        /// Направить для исполнения
        /// </summary>
        SendForExecution = 100032, 

        /// <summary>
        /// Отметить исполнение
        /// </summary>
        MarkExecution = 100035, 

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
    }
}