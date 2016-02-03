using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore.Actions
{
    public class ChangeFavourites
    {
        public int DocumentId { get; set; }
        public bool IsFavourite { get; set; }
    }
}
