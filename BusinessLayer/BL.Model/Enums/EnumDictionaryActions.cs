﻿namespace BL.Model.Enums
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
        DeleteCustomDictionary,
    }
}