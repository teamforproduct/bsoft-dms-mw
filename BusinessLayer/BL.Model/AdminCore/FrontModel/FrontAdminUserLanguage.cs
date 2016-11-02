using BL.Model.AdminCore.IncomingModel;

namespace BL.Model.AdminCore.FrontModel
{
    /// <summary>
    /// Модель языка
    /// </summary>
    public class FrontAdminUserLanguage: FrontAdminLanguage
    {

        /// <summary>
        /// Язык выбранный пользователем
        /// </summary>
        public bool? IsChecked { get; set; }
    }
}