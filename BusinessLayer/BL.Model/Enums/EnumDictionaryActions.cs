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
        AddContact = 207001,
        /// <summary>
        /// Изменить контакт
        /// </summary>
        ModifyContact = 207005,
        /// <summary>
        /// Удалить контакт
        /// </summary>
        DeleteContact = 207009,
        /// <summary>
        /// добавить тип контакта
        /// </summary>
        AddContactType = 205001,
        /// <summary>
        /// изменить тип контакта
        /// </summary>
        ModifyContactType = 205005,
        /// <summary>
        /// удалить тип контакта
        /// </summary>
        DeleteContactType = 205009,
        /// <summary>
        /// добавить адрес
        /// </summary>
        AddAgentAddress = 208001,
        /// <summary>
        /// изменить адрес
        /// </summary>
        ModifyAgentAddress = 208005,
        /// <summary>
        /// удалить адрес
        /// </summary>
        DeleteAgentAddress = 208009,
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
        /// <summary>
        /// добавить физлицо
        /// </summary>
        AddAgentPerson = 209001,
        /// <summary>
        /// изменить физлицо
        /// </summary>
        ModifyAgentPerson = 209005,
        /// <summary>
        /// удалить физлицо
        /// </summary>
        DeleteAgentPerson = 209009,
        /// <summary>
        /// добавить сотрудника
        /// </summary>
        AddAgentEmployee = 212001,
        /// <summary>
        /// изменить сотрудника
        /// </summary>
        ModifyAgentEmployee=212005,
        /// <summary>
        /// удалить сотрудника
        /// </summary>
        DeleteAgentEmployee=212009,
        /// <summary>
        /// добавить контраента
        /// </summary>
        AddAgent = 206001,
        /// <summary>
        /// изменить контрагента
        /// </summary>
        ModifyAgent = 206005,
        /// <summary>
        /// удалить контрагента
        /// </summary>
        DeleteAgent = 206009,

        AddRegistrationJournal,
        ModifyRegistrationJournal,
        DeleteRegistrationJournal,
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