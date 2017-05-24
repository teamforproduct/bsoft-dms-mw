using BL.Model.Common;
using BL.Model.Enums;
using BL.Model.Users;
using System.Runtime.Serialization;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Добавляемый или редактируемый файл документа или из веременного хранилища(TmpFileId) или из уже существующего файла (FileId)
    /// </summary>
    public class AddDocumentFile : CurrentPosition
    {
        /// <summary>
        /// Ид. документа, которому принадлежит файл
        /// </summary>
        public int DocumentId { get; set; }
        /// <summary>
        /// ИД файла во временном хранилище(или TmpFileId или FileId)
        /// </summary>        
        public int? TmpFileId { get; set; }
        /// <summary>
        /// ИД файла который надо скопировать (или TmpFileId или FileId)
        /// </summary>        
        public int? CopyingFileId { get; set; }
        /// <summary>
        /// Порядковый номер файла в списке файлов документа, если указан, то файл будет добавлен как версия, если нет, то добавлен как новый файл. Если файл с таким названием есть, то будет переименован.
        /// </summary>
        public int? OrderInDocument { get; set; }        
        /// <summary>
        /// Является ли файл дополнительным или основным.
        /// </summary>
        public EnumFileTypes Type { get; set; }      
        /// <summary>
        /// Признак основная версия файла. Для нового файла будет автоматом true. Для версии можно указать, но будут проверки.
        /// </summary>
        public bool? IsMainVersion { get; set; }
        /// <summary>
        /// Описание файла
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// ИД должности ответсвенного за файл
        /// </summary>
        [IgnoreDataMember]
        public int? ExecutorPositionId { get; set; }        
        /// <summary>
        /// ИД события к которому привязывать файл
        /// </summary>
        [IgnoreDataMember]
        public int? EventId { get; set; }
        /// <summary>
        /// Файл, который будт загружен из временного хранилища
        /// </summary>
        [IgnoreDataMember]
        public BaseFile File { get; set; }

    }
}
