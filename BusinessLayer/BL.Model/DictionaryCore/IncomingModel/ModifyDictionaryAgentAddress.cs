using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// адреса контрагентов
    /// </summary>
    public class ModifyDictionaryAgentAddress : AddDictionaryAgentAddress
    {
        /// <summary>
        /// ИД 
        /// </summary>
        public int Id { get; set; }

    }
}
