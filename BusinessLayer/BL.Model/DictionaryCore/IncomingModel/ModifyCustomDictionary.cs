using BL.Model.Users;
using System;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.IncomingModel
{
    public class ModifyCustomDictionary
    {
        /// <summary>
        /// ID
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }
        /// <summary>
        /// Ид. словаря
        /// </summary>
        public int DictionaryTypeId { get; set; }
        /// <summary>
        /// Код значения словаря
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Описание значения словаря
        /// </summary>
        public string Description { get; set; }
    }
}
