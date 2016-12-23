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
    public class AddAgentEmployee : AddAgentPerson
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
        public int LanguageId { get; set; }

    }
}
