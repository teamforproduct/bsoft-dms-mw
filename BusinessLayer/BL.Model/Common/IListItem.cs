using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.Common
{
    public interface IListItem
    {
        int Id { get; set; }
        string Name { get; set; }
    }

}
