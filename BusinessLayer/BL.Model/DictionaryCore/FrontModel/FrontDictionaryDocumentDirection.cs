﻿using System;

namespace BL.Model.DictionaryCore.FrontModel
{

    /// <summary>
    /// Направления документов
    /// </summary>
    public class FrontDictionaryDocumentDirection
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Код
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Пользователь
        /// </summary>
        public int LastChangeUserId { get; set; }
        /// <summary>
        /// Время последнего изменения
        /// </summary>
        public DateTime LastChangeDate { get; set; }
    }
}