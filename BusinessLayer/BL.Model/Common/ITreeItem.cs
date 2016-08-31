using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.Common
{
    public interface ITreeItem: IListItem
    {
    
        int? ParentItemId { get;  }

        int? ObjectId { get;  }

        bool IsUsed { get; set; }

        IEnumerable<ITreeItem> Childs { get; set; }
    }
}
