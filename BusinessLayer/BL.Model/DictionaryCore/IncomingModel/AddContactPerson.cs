using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DictionaryCore.IncomingModel
{
    public class AddContactPerson
    {
        /// <summary>
        /// Компания юридическое лицо
        /// </summary>
        [Required]
        public int CompanyId { get; set; }

        /// <summary>
        /// Физ.лицо
        /// </summary>
        [Required]
        public int PersonId { get; set; }

    }
}
