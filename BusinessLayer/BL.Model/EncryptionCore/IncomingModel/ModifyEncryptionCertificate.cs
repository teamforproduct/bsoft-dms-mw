using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.EncryptionCore.IncomingModel
{
    /// <summary>
    /// Модель для модификации документа
    /// </summary>
    public class ModifyEncryptionCertificate
    {
        /// <summary>
        /// ИД Документа
        /// </summary>
        [XmlIgnore]
        [Required]
        public int Id { get; set; }
        /// <summary>
        /// Название сертификата
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ID Агента. Используется только админом
        /// </summary>
        public int? AgentId { get; set; }
    }
}
