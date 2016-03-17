using System.Runtime.Serialization;

namespace BL.Model.AdminCore.IncomingModel
{
    /// <summary>
    /// Модель для добавления/редактирования
    /// </summary>
    public class ModifyAdminLanguageValue
    {
        /// <summary>
        /// ID
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }
        /// <summary>
        /// ID языка
        /// </summary>
        public int LanguageId { get; set; }
        /// <summary>
        /// Метка
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// Значение
        /// </summary>
        public string Value { get; set; }
    }
}
