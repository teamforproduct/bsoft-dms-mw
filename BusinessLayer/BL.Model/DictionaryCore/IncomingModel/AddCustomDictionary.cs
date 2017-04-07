﻿using System.ComponentModel.DataAnnotations;

namespace BL.Model.DictionaryCore.IncomingModel
{
    public class AddCustomDictionary
    {
        /// <summary>
        /// Ид. словаря
        /// </summary>
        [Required]
        public int TypeId { get; set; }

        /// <summary>
        /// Код значения словаря
        /// </summary>
        [Required]
        public string Code { get; set; }
        /// <summary>
        /// Наименование
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Описание значения словаря
        /// </summary>
        public string Description { get; set; }
    }
}
