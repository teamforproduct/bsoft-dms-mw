using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontDocumentInfo : FrontRegistrationFullNumber
    {
        public int? DocumentId { get; set; }
        public DateTime? DocumentDate { get; set; }
        public string DocumentDirectionName { get; set; }
        public string DocumentTypeName { get; set; }
        public string DocumentDescription { get; set; }
    }
}
