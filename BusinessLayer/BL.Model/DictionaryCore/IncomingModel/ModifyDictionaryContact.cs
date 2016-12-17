using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Enums;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// Контакт агента
    /// </summary>
    public class ModifyDictionaryAgentContact : AddDictionaryAgentContact
    {
        /// <summary>
        /// ИД
        /// </summary>
        [Required]
        public int Id { get; set; }

    }
}
