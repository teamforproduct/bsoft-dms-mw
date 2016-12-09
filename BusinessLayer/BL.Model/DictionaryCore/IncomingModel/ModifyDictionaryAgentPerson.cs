using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using BL.Model.Extensions;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// Контрагент - физическое лицо
    /// </summary>
    public class ModifyDictionaryAgentPerson : AddDictionaryAgentPerson
    {
        /// <summary>
        /// ID
        /// </summary>
        // TODO
        //[Required]
        public int Id { get; set; }

    }
}
