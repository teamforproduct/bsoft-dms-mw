using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore.FrontModel
{   
    /// <summary>
    /// Банк
    /// </summary>
    public class FrontMainAgentBank 
    {
        /// <summary>
        /// Ид
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Краткое название
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Полное название
        /// </summary>
        public string FullName { get; set; }
        
        /// <summary>
        /// МФО
        /// </summary>
        public string MFOCode { get; set; }
        
        /// <summary>
        /// Код Свифт
        /// </summary>
        public string Swift { get; set; }
        
        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Признак активности
        /// </summary>
        public bool? IsActive { get; set; }

    }
}
