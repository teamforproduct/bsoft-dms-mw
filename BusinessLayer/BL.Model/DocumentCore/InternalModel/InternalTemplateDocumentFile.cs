using System;
using BL.Model.Common;
using BL.Model.Enums;
using BL.Model.DocumentCore.FrontModel;

namespace BL.Model.DocumentCore.InternalModel
{
    /// <summary>
    /// Внутренний класс для работы с файлами, прикрепленными к шаблону документа
    /// </summary>
    public class InternalTemplateDocumentFile : LastChangeInfo
    {
        public InternalTemplateDocumentFile()
        {
        }

        public InternalTemplateDocumentFile(FrontTemplateDocumentFile doc)
        {
            Description = doc.Description;
            DocumentId = doc.DocumentId;
            Id = doc.Id;
            OrderInDocument = doc.OrderInDocument;
            File = doc.File;            
        }
        /// <summary>
        /// ИД.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ИД Документа
        /// </summary>
        public int DocumentId { get; set; }

        /// <summary>
        /// Номер вложения в документе
        /// </summary>
        public int OrderInDocument { get; set; }
        /// <summary>
        /// Признак дополнительный файл или основной
        /// </summary>
        public EnumFileTypes Type { get; set; }
        /// <summary>
        /// Признак дополнительный файл или основной
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Описание файла
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Файл
        /// </summary>
        public BaseFile File { get; set; }
 
        /// <summary>
        /// Хэш файла. для проверки целостности файла в хранилище
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// Был ли создан пдф файл на основании данного файла
        /// </summary>
        public bool PdfCreated { get; set; }

        /// <summary>
        /// Время последнего обращения к пдф файлу.
        /// </summary>
        public DateTime? LastPdfAccess { get; set; }

        /// <summary>
        /// Может ли файл вообще быть конвертированный в ПДФ
        /// </summary>
        public bool PdfAcceptable { get; set; }
    }
}