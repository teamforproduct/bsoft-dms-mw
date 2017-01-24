using BL.Model.Enums;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Web;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Изменение файла, прикрепленного к шаблону документа
    /// </summary>
    public class AddTemplateAttachedFile
    {
        /// <summary>
        /// Ид. шаблона документа, которому принадлежит файл
        /// </summary>
        [Required]
        public int DocumentId { get; set; }
        /// <summary>
        /// Является ли файл дополнительным или основным. 
        /// </summary>
        public EnumFileTypes Type { get; set; }
        /// <summary>
        /// Описание файла
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Имя файла. Включая расширение
        /// </summary>
        [IgnoreDataMember]
        public string FileName { get; set; }
        /// <summary>
        /// Тип файла.
        /// </summary>
        [IgnoreDataMember]
        public string FileType { get; set; }
        /// <summary>
        /// Размер файла
        /// </summary>
        [IgnoreDataMember]
        public int FileSize { get; set; }

        /// <summary>
        /// Данные файла
        /// </summary>
        [IgnoreDataMember]
        public HttpPostedFile PostedFileData { get; set; }
    }
}
