using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore.Actions
{
    public class ChangeWorkStatus
    {
        public int Id { get; set; }
        public bool IsInWork { get; set; }
        public string Description { get; set; }
     
    }
}
