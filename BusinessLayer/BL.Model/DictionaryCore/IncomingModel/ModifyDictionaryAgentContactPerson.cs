using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DictionaryCore.IncomingModel
{
    public class ModifyDictionaryAgentContactPerson : AddDictionaryAgentContactPerson
    {
        /// <summary>
        /// Ид
        /// </summary>
        [Required]
        public int Id { get; set; }

    }
}
