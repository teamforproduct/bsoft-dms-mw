namespace BL.Model.Enums
{
    /// <summary>
    /// Список экшенов по словарям
    /// </summary>
    public enum EnumDictionaryActions
    {
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
        /// <summary>
        /// Добавить контакт
        /// </summary>
        AddContact,
        /// <summary>
        /// Изменить контакт
        /// </summary>
        ModifyContact,
        /// <summary>
        /// Удалить контакт
        /// </summary>
        DeleteContact,

 		AddContactType,
        ModifyContactType,
        DeleteContactType,
        AddAgentAddress,
        ModifyAgentAddress,
        DeleteAgentAddress,
        #region DictionaryDocumentSubject
        /// <summary>
        /// Добавить тип адреса
        /// </summary>
        AddDocumentSubject = 203001,
        /// <summary>
        /// Изменить тип адреса
        /// </summary>       
        ModifyDocumentSubject = 203005,
        /// <summary>
        /// Удалить тип адреса
        /// </summary>       
        DeleteDocumentSubject = 203009,
        #endregion DictionaryDocumentSubject
        AddAgentPerson,
        ModifyAgentPerson,
        DeleteAgentPerson,

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

        /// <summary>
        /// Добавить тип пользовательского словаря
        /// </summary>
        AddCustomDictionaryType,
        /// <summary>
        /// Редактировать тип пользовательского словаря
        /// </summary>
        ModifyCustomDictionaryType,
        /// <summary>
        /// Удалить тип пользовательского словаря
        /// </summary>
        DeleteCustomDictionaryType,
        /// <summary>
        /// Добавит пользовательсткий словать
        /// </summary>
        AddCustomDictionary,
        /// <summary>
        /// Редактировать пользовательский словарь
        /// </summary>
        ModifyCustomDictionary,
        /// <summary>
        /// Удалить пользовательский словарь
        /// </summary>
        DeleteCustomDictionary
    }
}