using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore.IncomingModel
{
    public class AddTemplateDocumentTask
    {
        public int DocumentId { get; set; }
        public string Task { get; set; }
        public string Description { get; set; }
    }
}
