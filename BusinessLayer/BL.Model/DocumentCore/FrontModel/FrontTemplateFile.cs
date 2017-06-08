using BL.Model.DocumentCore.InternalModel;

namespace BL.Model.DocumentCore.FrontModel
{
    /// <summary>
    /// Класс для отображения файлов, прикрепленных к шаблону документу
    /// </summary>
    public class FrontTemplateFile: InternalTemplateFile
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public FrontTemplateFile()
        {
        }

        /// <summary>
        /// конструктор для превращения внутренней модели файла в модель отображения для пользователя
        /// </summary>
        /// <param name="doc"></param>
        public FrontTemplateFile(InternalTemplateFile doc)
        {
            Id = doc.Id;
            DocumentId = doc.DocumentId;
            OrderInDocument = doc.OrderInDocument;
            File = doc.File;
            //FileContent = doc.FileContent;
            //Name = doc.Name;
            //Extension = doc.Extension;
            //FileType = doc.FileType;
            //FileSize = doc.FileSize;
            Type = doc.Type;
            TypeName = doc.TypeName;
            Hash = doc.Hash;
            Description = doc.Description;
        }

        /// <summary>
        /// Имя пользователя, который последний редактировал файл
        /// </summary>
        public string LastChangeUserName { get; set; }
    }
}