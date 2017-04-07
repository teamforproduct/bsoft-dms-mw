using BL.Model.DictionaryCore.IncomingModel;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Контрагент - сотрудник
    /// </summary>
    public class FrontAgentEmployee: ModifyAgentEmployee
    {
        /// <summary>
        /// Имя
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Полное имя
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Наименование языка
        /// </summary>
        public string LanguageName { get; set; }

        /// <summary>
        /// Код языка
        /// </summary>
        public string LanguageCode { get; set; }

        /// <summary>
        /// Аватарка
        /// </summary>
        public string Image { get { return Converter.ToBase64String(imageByteArray); } }

        [IgnoreDataMember]
        public byte[] ImageByteArray { set { imageByteArray = value; } }
        private byte[] imageByteArray;
    }


}
