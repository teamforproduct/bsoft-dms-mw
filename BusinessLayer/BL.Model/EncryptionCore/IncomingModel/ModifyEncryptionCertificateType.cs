using System.Runtime.Serialization;

namespace BL.Model.EncryptionCore.IncomingModel
{
    /// <summary>
    /// Модель для добавления/редактирования
    /// </summary>
    public class ModifyEncryptionCertificateType
    {
        /// <summary>
        /// ID
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }
        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Код
        /// </summary>
        public string Code { get; set; }
    }
}
