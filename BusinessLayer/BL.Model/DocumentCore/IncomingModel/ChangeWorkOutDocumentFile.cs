using BL.Model.Users;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Модель для изменения признака принятия файла
    /// </summary>
    public class ChangeWorkOutDocumentFile
    {
        /// <summary>
        /// ИД документа
        /// </summary>
        [Required]
        public int DocumentId { get; set; }
        /// <summary>
        /// Номер вложения в документе
        /// </summary>
        [Required]
        public int OrderInDocument { get; set; }
        /// <summary>
        /// Версия вложения
        /// </summary>
        [Required]
        public int Version { get; set; }
    }
}
