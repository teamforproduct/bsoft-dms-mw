using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.Common
{
    public class ListItem : IListItem
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
