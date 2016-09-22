using System.Xml.Serialization;
using System.Runtime.Serialization;

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
        [IgnoreDataMember]
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
