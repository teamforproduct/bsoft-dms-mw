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

        public int? ParentId { get; set; }

        public int ObjectId { get; set; }

        public string Name { get; set; }

        public ITreeItem Parent { get; set; }

        public IEnumerable<ITreeItem> Childs { get; set; }
    }
}
