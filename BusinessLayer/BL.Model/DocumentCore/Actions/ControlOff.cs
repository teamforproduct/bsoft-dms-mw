using BL.Model.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore.Actions
{
    public class ControlOff : CurrentPosition
    {
        public int DocumentId { get; set; }
        public int ResultTypeId { get; set; }
        public string Description { get; set; }
        public bool IsCascade { get; set; }
    }
}
