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
    /// Агент
    /// </summary>
    public class AddAgent
    {
        /// <summary>
        /// Краткое название/имя (отображается в интерфейсе как основное)
        /// </summary>
        [Required]
        public string Name { get; set; }

    }
}
