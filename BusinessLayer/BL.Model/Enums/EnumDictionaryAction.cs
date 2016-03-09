namespace BL.Model.Enums
{
    public enum EnumDictionaryAction
    {
        AddDocumentType = 201001, // Добавить тип документа
        ModifyDocumentType = 201005, // Изменить тип документа
        DeleteDocumentType = 201009, // Удалить тип документа

        AddAddressType,  // Добавить тип адреса
        ModifyAddressType,  // Изменить тип адреса
        DeleteAddressType,  // Удалить тип адреса


        AddTag = 291001, // Добавить тэг
        ModifyTag = 291005, // Изменить тэг
        DeleteTag = 291009, // Удалить тэг

        AddCustomDictionaryType,
        ModifyCustomDictionaryType,
        DeleteCustomDictionaryType,
        AddCustomDictionary,
        ModifyCustomDictionary,
        DeleteCustomDictionary,
    }
}