using BL.Model.Users;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.IncomingModel
{
    public class AddAgentFavourite
    {
        /// <summary>
        /// Ид. объекта
        /// </summary>
        [Required]
        public int ObjectId { get; set; }

        /// <summary>
        /// Модуль
        /// </summary>
        [Required]
        public string Module { get; set; }
        /// <summary>
        /// Фича
        /// </summary>
        [Required]
        public string Feauture { get; set; }
    }
}
