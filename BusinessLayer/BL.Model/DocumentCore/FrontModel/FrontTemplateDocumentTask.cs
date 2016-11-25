using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontTemplateDocumentTask
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public string Task { get; set; }
        public string Description { get; set; }
    }
}
