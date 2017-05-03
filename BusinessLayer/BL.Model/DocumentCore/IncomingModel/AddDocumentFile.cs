using BL.Model.Common;
using BL.Model.Enums;
using BL.Model.Users;
using System.Runtime.Serialization;
using System.Web;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Добавляемый или редактируемый файл документа
    /// </summary>
    public class AddDocumentFile : CurrentPosition
    {
        /// <summary>
        /// Ид. документа, которому принадлежит файл
        /// </summary>
        public int DocumentId { get; set; }
        
        /// <summary>
        /// Порядковый номер файла в списке файлов документа
        /// Только для добавления версии файла к файлу
        /// </summary>
        public int OrderInDocument { get; set; }
        
        /// <summary>
        /// Является ли файл дополнительным или основным.
        /// </summary>
        public EnumFileTypes Type { get; set; }

        [IgnoreDataMember]
        public bool IsUseMainNameFile { get; set; }
        
        ///// <summary>
        ///// Имя файла.
        ///// </summary>
        //[IgnoreDataMember]
        //public string FileName { get; set; }
        
        ///// <summary>
        ///// Тип файла.
        ///// </summary>
        //[IgnoreDataMember]
        //public string FileType { get; set; }
        
        /// <summary>
        /// Описание файла
        /// </summary>
        public string Description { get; set; }
        
        ///// <summary>
        ///// Размер файла
        ///// </summary>
        //[IgnoreDataMember]
        //public long FileSize { get; set; }

        ///// <summary>
        ///// Данные файла
        ///// </summary>
        //[IgnoreDataMember]
        //public HttpPostedFile PostedFileData { get; set; }
        /// <summary>
        /// Файл
        /// </summary>
        //[IgnoreDataMember]
        //public BaseFile File { get; set; }
        /// <summary>
        /// ИД файла, сохраненного во временном хранилище
        /// </summary>
        public int TmpFileId { get; set; }
    }
}
