namespace BL.Model.Enums
{
    public enum EnumObjects
    {
        Documents = 100, // Документы
        DocumentAccesses = 101, // Документы - доступы
        DocumentRestrictedSendLists = 102, // Документы - ограничения рассылки
        DocumentSendLists = 103, // Документы - план работы
        DocumentFiles = 104, // Документы - файлы
        DocumentLinks = 105, // Документы - связи
        DocumentEvents = 111, // Документы - события
        DocumentWaits = 112, // Документы - ожидания
        DocumentSubscriptions = 113, // Документы - подписи
        DocumentSavedFilters = 191, // Документы - сохраненные фильтры
        DocumentTags = 192, // Документы - тэги
        DictionaryDocumentType = 201, // Типы документов
        DictionaryTag = 291, // Теги
        CustomDictionaryTypes = 301, // Типы пользовательских словарей
        CustomDictionaries = 302, // Пользовательские словари
        Properties = 311, // Динамические аттрибуты
        PropertyLinks = 312, // Связи динамических аттрибутов с объектами системы
        PropertyValues = 313, // Значения динамических аттрибутов
    }
}