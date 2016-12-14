using BL.Model.Extensions;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Web;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// контрагент - сотрудник
    /// </summary>
    public class ModifyDictionaryAgentEmployee : AddDictionaryAgentEmployee
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required]
        public int Id { get; set; }

    }
}
