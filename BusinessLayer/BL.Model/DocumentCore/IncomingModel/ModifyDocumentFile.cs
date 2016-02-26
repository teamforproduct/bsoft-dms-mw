using System.Runtime.Serialization;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Добавляемый или редактируемый файл документа
    /// </summary>
    public class ModifyDocumentFile
    {
        [IgnoreDataMember]
        public int Id { get; set; }
        /// <summary>
        /// Ид. документа, которому принадлежит файл
        /// </summary>
        public int DocumentId { get; set; }
        /// <summary>
        /// Порядковый номер файла в списке файлов документа
        /// </summary>
        public int OrderInDocument { get; set; }
        /// <summary>
        /// Является ли файл дополнительным или основным. 
        /// </summary>
        public bool IsAdditional { get; set; }
        /// <summary>
        /// Данные файла в виде строки
        /// </summary>
        public string FileData { get; set; }
        /// <summary>
        /// Имя файла. Включая расширение
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Тип файла.
        /// </summary>
        public string FileType { get; set; }
        /// <summary>
        /// Размер файла
        /// </summary>
        public int FileSize { get; set; }
    }
}
