using BL.Model.Users;
using System.Runtime.Serialization;
using System.Web;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Добавляемый или редактируемый файл документа
    /// </summary>
    public class ModifyDocumentFile : CurrentPosition
    {
        [IgnoreDataMember]
        public int Id { get; set; }
        /// <summary>
        /// Ид. документа, которому принадлежит файл
        /// </summary>
        public int DocumentId { get; set; }
        /// <summary>
        /// Порядковый номер файла в списке файлов документа
        /// Только для изменения файла и для добавления версию файла к файлу
        /// </summary>
        public int OrderInDocument { get; set; }
        /// <summary>
        /// Версия файла
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        /// Является ли файл дополнительным или основным.
        /// Только для добавления файла
        /// </summary>
        public bool IsAdditional { get; set; }
        [IgnoreDataMember]
        public bool IsUseMainNameFile { get; set; }
        /// <summary>
        /// Имя файла. Включая расширение
        /// Только для изменения файла
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Тип файла.
        /// Только для изменения файла
        /// </summary>
        public string FileType { get; set; }
        /// <summary>
        /// Описание файла
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Размер файла
        /// </summary>
        [IgnoreDataMember]
        public long FileSize { get; set; }

        /// <summary>
        /// Данные файла
        /// </summary>
        [IgnoreDataMember]
        public HttpPostedFile PostedFileData { get; set; }
    }
}
