using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.Common
{
    public interface ITreeItem: IListItem
    {
    
        int? ParentId { get;  }

        int ObjectId { get;  }

        ITreeItem Parent { get; }

        IEnumerable<ITreeItem> Childs { get; }
    }
}
