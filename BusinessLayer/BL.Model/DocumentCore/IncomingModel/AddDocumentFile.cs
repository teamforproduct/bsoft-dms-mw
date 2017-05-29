using BL.Model.Common;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Users;
using System.Runtime.Serialization;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Добавляемый или редактируемый файл документа  (один из вариантов!)
    /// - из веременного хранилища(TmpFileId)
    /// - из уже существующего файла (CopyingFileId)
    /// - переместить в другой файл как версию (возможно с заменой исполнителя) (MovingFileId)
    /// - добавить ссылку на существующий файл (LinkingFileId)
    /// </summary>
    public class AddDocumentFile : CurrentPosition
    {
        /// <summary>
        /// Ид. документа, с которым связана работа с файлом
        /// </summary>
        public int DocumentId { get; set; }
        /// <summary>
        /// ИД файла во временном хранилище для добавления
        /// </summary>        
        public int? TmpFileId { get; set; }
        /// <summary>
        /// ИД файла, который надо скопировать для добавления
        /// </summary>        
        public int? CopyingFileId { get; set; }
        /// <summary>
        /// ИД файла, который надо переместить в другой файл как версию (возможно с заменой исполнителя)
        /// </summary>        
        public int? MovingFileId { get; set; }
        /// <summary>
        /// ИД файла, на который нужно сделать ссылку, в создаваемом евенте (или TmpFileId, или CopyingFileId, или LinkingFileId)
        /// </summary>        
        public int? LinkingFileId { get; set; }
        /// <summary>
        /// При добавлении файла порядковый номер файла в списке файлов документа, 
        /// если указан, то файл будет добавлен как версия, 
        /// если нет, то добавлен как новый файл. 
        /// для MovingFileId обязателен
        /// </summary>
        public int? OrderInDocument { get; set; }
        /// <summary>
        /// При добавлении файла указание является ли файл дополнительным или основным.
        /// </summary>
        public EnumFileTypes? Type { get; set; }
        /// <summary>
        /// При добавлении файла указание признака основная версия файла. 
        /// Для нового файла будет автоматом true. 
        /// Для версии можно указать, но будут проверки, является ли источник владельцем файла.
        /// </summary>
        public bool? IsMainVersion { get; set; }
        /// <summary>
        /// При добавлении файла опциональное указание описания файла
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Признак отработки всех версий файлов при копировании или перемещении (CopyingFileId,MovingFileId)
        /// Только если  CopyingFileId или MovingFileId - основной файл
        /// </summary>
        public bool? IsAllVersionsProcessing { get; set; }
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
        /// Модель Файла, который будет обработан (вместе с самим файлом)
        /// </summary>
        [IgnoreDataMember]
        public InternalDocumentFile File { get; set; }

    }
}
