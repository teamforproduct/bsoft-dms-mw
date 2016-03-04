using BL.Model.Users;
using System;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.IncomingModel
{
    public class ModifyCustomDictionaryType
    {
        /// <summary>
        /// ID
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }
        /// <summary>
        /// Код словаря
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Описание словаря
        /// </summary>
        public string Description { get; set; }
    }
}
