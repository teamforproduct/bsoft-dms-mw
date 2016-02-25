using BL.Model.DocumentCore.InternalModel;

namespace BL.Model.DocumentCore.FrontModel
{
    /// <summary>
    /// Класс для отображения файлов, прикрепленных к шаблону документу
    /// </summary>
    public class FrontTemplateAttachedFile: InternalTemplateAttachedFile
    {
        /// <summary>
        /// Имя пользователя, который последний редактировал файл
        /// </summary>
        public string LastChangeUserName { get; set; }
    }
}