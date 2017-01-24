using BL.Model.DocumentCore.InternalModel;

namespace BL.Model.DocumentCore.FrontModel
{
    /// <summary>
    /// Класс для отображения файлов, прикрепленных к шаблону документу
    /// </summary>
    public class FrontTemplateAttachedFile: InternalTemplateAttachedFile
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public FrontTemplateAttachedFile()
        {
        }

        /// <summary>
        /// конструктор для превращения внутренней модели файла в модель отображения для пользователя
        /// </summary>
        /// <param name="doc"></param>
        public FrontTemplateAttachedFile(InternalTemplateAttachedFile doc)
        {
            Id = doc.Id;
            DocumentId = doc.DocumentId;
            OrderInDocument = doc.OrderInDocument;
            FileContent = doc.FileContent;
            Name = doc.Name;
            Extension = doc.Extension;
            FileType = doc.FileType;
            FileSize = doc.FileSize;
            Type = doc.Type;
            TypeName = doc.TypeName;
            Hash = doc.Hash;
            Description = doc.Description;
        }

        /// <summary>
        /// Имя пользователя, который последний редактировал файл
        /// </summary>
        /// <summary>
        /// Имя пользователя, который последний редактировал файл
        /// </summary>
        public string LastChangeUserName { get; set; }
    }
}