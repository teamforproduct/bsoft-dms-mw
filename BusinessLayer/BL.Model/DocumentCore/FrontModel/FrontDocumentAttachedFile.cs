using BL.Model.DocumentCore.InternalModel;

namespace BL.Model.DocumentCore.FrontModel
{
    /// <summary>
    /// Модель файла документа для отображения пользователю
    /// </summary>
    public class FrontDocumentAttachedFile: InternalDocumentAttachedFile
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public FrontDocumentAttachedFile()
        {
        }

        /// <summary>
        /// конструктор для превращения внутренней модели файла в модель отображения для пользователя
        /// </summary>
        /// <param name="doc"></param>
        public FrontDocumentAttachedFile(InternalDocumentAttachedFile doc)
        {
            Id = doc.Id;
            DocumentId = doc.DocumentId;
            OrderInDocument = doc.OrderInDocument;

            Version = doc.Version;
            FileContent = doc.FileContent;
            Name = doc.Name;
            Extension = doc.Extension;

            FileType = doc.FileType;
            FileSize = doc.FileSize;
            IsAdditional = doc.IsAdditional;
            Date = doc.Date;
            Hash = doc.Hash;
            WasChangedExternal = doc.WasChangedExternal;
        }

        /// <summary>
        /// Имя пользователя, который последний редактировал файл
        /// </summary>
        public string LastChangeUserName { get; set; }
    }
}