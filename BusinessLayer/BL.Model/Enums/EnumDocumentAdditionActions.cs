namespace BL.Model.Enums
{
    /// <summary>
    /// 
    /// </summary>
    public enum EnumDocumentAdditionActions
    {
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
        /// Добавить план работы по стандартному списку
        /// </summary>
        AddByStandartSendListDocumentSendList = 103002, 

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
        /// Добавить связь между документами
        /// </summary>
        AddDocumentLink = 105001, 

        /// <summary>
        /// Удалить связь между документами
        /// </summary>
        DeleteDocumentLink = 105009, 

        /// <summary>
        /// Изменить тэги по документу
        /// </summary>
        ModifyDocumentTags = 192005, 
    }
}