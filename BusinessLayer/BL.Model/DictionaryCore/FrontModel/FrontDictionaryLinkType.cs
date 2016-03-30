﻿using System;

namespace BL.Model.DictionaryCore.FrontModel
{

    /// <summary>
    /// Типы связей документов
    /// </summary>
    public class FrontDictionaryLinkType
    {
        /// <summary>
        /// ИД
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Является важной
        /// </summary>
        public bool IsImportant { get; set; }
        /// <summary>
        /// Пользователь
        /// </summary>
        public int LastChangeUserId { get; set; }
        /// <summary>
        /// Дата изменения
        /// </summary>
        public DateTime LastChangeDate { get; set; }
    }
}