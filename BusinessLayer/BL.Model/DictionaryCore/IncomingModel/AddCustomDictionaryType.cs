using BL.Model.Users;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.IncomingModel
{
    public class AddCustomDictionaryType
    {

        /// <summary>
        /// Код словаря
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Описание словаря
        /// </summary>
        public string Description { get; set; }
    }
}
