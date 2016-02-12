using BL.Model.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore.Actions
{
    public class CopyDocument : CurrentPosition
    {
        public int DocumentId { get; set; }
    }
}
