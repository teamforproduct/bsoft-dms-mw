using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Common;

namespace BL.Model.DictionaryCore.InternalModel
{
    public class InternalDictionaryAgent : LastChangeInfo
    {

        public int Id { get; set; }
        public string Name { get; set; }

        public bool IsCompany { get; set; }
        public bool IsIndividual { get; set; }
        public bool IsEmployee { get; set; }
        public bool IsBank { get; set; }

        public bool IsActive { get; set; }

        public int ResidentTypeId { get; set; }
        public string Description { get; set; }
    }
}
