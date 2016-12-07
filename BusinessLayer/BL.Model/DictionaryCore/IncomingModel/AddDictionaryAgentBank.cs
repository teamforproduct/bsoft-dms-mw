using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DictionaryCore.IncomingModel
{
    public class AddDictionaryAgentBank
    {
        /// <summary>
        /// МФО
        /// </summary>
        [Required]
        public string MFOCode { get; set; }

        /// <summary>
        /// Код Свифт
        /// </summary>
        public string Swift { get; set; }

        /// <summary>
        /// Комментарии
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Признак активности
        /// </summary>
        [Required]
        public bool IsActive { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Полное имя
        /// </summary>
        [Required]
        public string FullName { get; set; }

    }
}
