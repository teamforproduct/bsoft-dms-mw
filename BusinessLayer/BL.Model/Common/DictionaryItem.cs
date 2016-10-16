using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.Common
{
    public class DictionaryItem : ListItem, IDictionaryItem
    {
        public string Description { get; set; }

        public bool IsActive { get; set; }
    }
}
