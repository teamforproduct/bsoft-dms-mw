using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DictionaryCore.IncomingModel
{
    public class ModifyDictionaryAgentBank : AddDictionaryAgentBank
    {
        /// <summary>
        /// Ид
        /// </summary>
        //[Required]
        public int Id { get; set; }

    }
}
