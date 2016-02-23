namespace BL.Model.Enums
{
    public enum EnumDocumentAdditionActions
    {
        AddDocumentRestrictedSendList = 102001, // Добавить ограничение рассылки
        AddByStandartSendListDocumentRestrictedSendList = 102002, // Добавить ограничения рассылки по стандартному списку
        DeleteDocumentRestrictedSendList = 102009, // Удалить ограничение рассылки
        AddDocumentSendList = 103001, // Добавить пункт плана
        AddByStandartSendListDocumentSendList = 103002, // Добавить план работы по стандартному списку
        ModifyDocumentSendList = 103005, // Изменить документ
        DeleteDocumentSendList = 103009, // Удалить пункт плана
        AddDocumentSendListStage = 103011, // Добавить этап плана
        DeleteDocumentSendListStage = 103019, // Удалить этап плана
        AddDocumentFile = 104001, // Добавить файл
        ModifyDocumentFile = 104005, // Изменить файл
        DeleteDocumentFile = 104009, // Удалить файл
        AddDocumentLinks = 105001, // Добавить связь между документами
        DeleteDocumentLinks = 105009, // Удалить связь между документами
        ModifyDocumentTags = 192005, // Изменить тэги по документу
    }
}