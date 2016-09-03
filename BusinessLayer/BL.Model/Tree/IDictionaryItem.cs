using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.Tree
{
    public interface IDictionaryItem : IListItem
    {
        bool IsActive { get; set; }
        string Description { get; set; }
    }

}
