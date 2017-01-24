using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Web;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// Контрагент - физическое лицо
    /// </summary>
    public class ModifyDictionaryAgentImage
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Данные файла
        /// </summary>
        [IgnoreDataMember]
        public string PostedFileData { get; set; }

    }
}