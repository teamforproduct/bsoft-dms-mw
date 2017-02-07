using BL.Model.Common;

namespace BL.Model.AdminCore.InternalModel
{
    public class InternalAdminLanguage : ListItem
    {
        /// <summary>
        /// Код языка
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Язык по умолчанию
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Наименование файла
        /// </summary>
        public string FileName { get; set; }
    }
}