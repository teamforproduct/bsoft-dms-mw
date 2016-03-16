namespace BL.Model.Enums
{
    /// <summary>
    /// Список экшенов по словарям
    /// </summary>
    public enum EnumDictionaryActions
    {
        // DocumentType
        #region DictionaryDocumentType
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
        #endregion DictionaryDocumentType

        // AddressType
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
        
        // Contact
        #region Contact
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
        #endregion Contact

        // ContactType
        #region ContactType
        AddContactType = 205001,
        ModifyContactType = 205005,
        DeleteContactType = 205009,
		#endregion ContactType

		// AgentAddress
        #region AgentAddress
        AddAgentAddress =208001,
        ModifyAgentAddress=208005,
        DeleteAgentAddress=208009,
        #endregion AgentAddress

        // AgentPerson,
        #region AgentPerson
        AddAgentPerson,
        ModifyAgentPerson,
        DeleteAgentPerson,
        #endregion AgentPerson

        // DocumentSubject
        #region DictionaryDocumentSubject
        /// <summary>
        /// Добавить тематику документа
        /// </summary>
        AddDocumentSubject = 203001,
        /// <summary>
        /// Изменить тематику документа
        /// </summary>       
        ModifyDocumentSubject = 203005,
        /// <summary>
        /// Удалить тематику документа
        /// </summary>       
        DeleteDocumentSubject = 203009,
        #endregion DictionaryDocumentSubject
        AddAgent = 206001,
        ModifyAgent = 206005,
        DeleteAgent = 206009,

        // RegistrationJournal
        #region DictionaryRegistrationJournal
        /// <summary>
        /// Добавить регистрационный журнал
        /// </summary>
        AddRegistrationJournal = 204001,
        /// <summary>
        /// Изменить регистрационный журнал
        /// </summary>       
        ModifyRegistrationJournal = 204005,
        /// <summary>
        /// Удалить регистрационный журнал
        /// </summary>       
        DeleteRegistrationJournal = 204009,
        #endregion DictionaryDocumentSubject

        // Tag
        #region Tag
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
        #endregion Tag

        // CustomDictionaryTyp
        #region CustomDictionaryTyp
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
        #endregion CustomDictionaryTyp

        // CustomDictionary
        #region CustomDictionary
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
        #endregion CustomDictionary
    }
}