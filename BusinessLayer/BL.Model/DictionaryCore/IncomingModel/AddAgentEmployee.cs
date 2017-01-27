using BL.Model.Extensions;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Web;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// контрагент - сотрудник
    /// </summary>
    public class AddAgentEmployee : AddAgentPeople
    {

        /// <summary>
        /// ИД аватарки, если она была загружена
        /// </summary>
        public int? ImageId { get; set; }

        /// <summary>
        /// Данные файла
        /// </summary>
        [IgnoreDataMember]
        public string PostedFileData { get; set; }

        /// <summary>
        /// табельный номер
        /// </summary>
        public int PersonnelNumber { get; set; }

        /// <summary>
        /// Профиль пользователя. Язык интерфейса.
        /// </summary>
        [Required]
        public int LanguageId { get; set; }

        /// <summary>
        /// Дополнительная информация
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///Признак активности
        /// </summary>
        [Required]
        public bool IsActive { get; set; }

    }
}
