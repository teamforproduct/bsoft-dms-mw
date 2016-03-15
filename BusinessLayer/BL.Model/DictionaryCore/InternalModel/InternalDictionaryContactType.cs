using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Common;

namespace BL.Model.DictionaryCore.InternalModel
{
    public class InternalDictionaryContactType : LastChangeInfo
    {
     
        public int Id { get; set; }
        
        public string Name { get; set; }
   
        public string InputMask { get; set; }

        public bool IsActive { get; set; }
    }
}
