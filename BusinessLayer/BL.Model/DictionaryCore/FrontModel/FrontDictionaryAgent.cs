using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore.FrontModel
{
    public class FrontDictionaryAgent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsIndividual { get; set; }
        public bool IsEmployee { get; set; }
        public bool IsBank { get; set; }
        public bool IsCompany { get; set; }
        public bool IsActive { get; set; }
        public int ResidentTypeId { get; set; }
        public string Description { get; set; }

        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
      
    }
}
