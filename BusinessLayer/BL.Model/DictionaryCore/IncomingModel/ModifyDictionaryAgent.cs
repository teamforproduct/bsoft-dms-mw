using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace BL.Model.DictionaryCore.IncomingModel
{
    public class ModifyDictionaryAgent
    {

        /// <summary>
        /// ИД
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }
        /// <summary>
        /// ФИО, Название, ...
        /// </summary>
        [Required]
        public string Name { get; set; }

       /// <summary>
       /// Резидентность
       /// </summary>
        public int? ResidentTypeId { get; set; }

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
        /// Дополнительная информация
        /// </summary>
        [System.Obsolete("Договорились не использовать", true)]
        public string Description { get; set; }
        /// <summary>
        /// Признак активности
        /// </summary>
        [System.Obsolete("Договорились не использовать", true)]
        public bool IsActive { get; set; }
       
    }
}
