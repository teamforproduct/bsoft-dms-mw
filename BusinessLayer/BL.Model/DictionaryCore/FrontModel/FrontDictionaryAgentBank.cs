using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore.FrontModel
{   
    /// <summary>
    /// Контрагент - Банк
    /// </summary>
    public class FrontDictionaryAgentBank : FrontDictionaryAgent
    {
        /// <summary>
        /// Ид
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Название
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
        /// Комментарии
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; set; }

    }
}
