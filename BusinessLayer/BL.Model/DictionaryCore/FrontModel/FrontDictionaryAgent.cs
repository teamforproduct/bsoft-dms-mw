using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Enums;

namespace BL.Model.DictionaryCore.FrontModel
{


    /// <summary>
    /// ОСНОВНОЙ. Справочник контрагентов
    /// </summary>
    public class FrontDictionaryAgent
    {
        /// <summary>
        /// ИД
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ФИО, Название, ...
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Кем в данный момент является контрагент
        /// </summary>
        public IEnumerable<EnumDictionaryAgentTypes> ActualTypes { get; set; }

        //public bool IsIndividual { get; set; }
        //public bool IsEmployee { get; set; }
        //public bool IsBank { get; set; }
        //public bool IsCompany { get; set; }
        //public bool IsActive { get; set; }

        /// <summary>
        /// Резидентность
        /// </summary>
        public int ResidentTypeId { get; set; }
        /// <summary>
        /// Дополнительная информация
        /// </summary>
        public string Description { get; set; }
      
    }
}
