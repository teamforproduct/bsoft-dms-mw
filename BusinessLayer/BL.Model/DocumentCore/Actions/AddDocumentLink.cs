using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore.Actions
{
    public class AddDocumentLink
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public int LinkTypeId { get; set; }       

    }
}
