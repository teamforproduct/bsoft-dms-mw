using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.Common
{
    public class TreeItem : ITreeItem
    {
        public int Id { get; set; }

        public int? ParentItemId { get; set; }

        public int? ObjectId { get; set; }

        public string Name { get; set; }

        public bool IsUsed { get; set; }

        public IEnumerable<ITreeItem> Childs { get; set; }
    }
}
