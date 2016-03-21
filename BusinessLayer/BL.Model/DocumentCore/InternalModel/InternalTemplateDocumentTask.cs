using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Common;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalTemplateDocumentTask : LastChangeInfo
    {

        public int Id { get; set; }
        public int DocumentId { get; set; }
        public Nullable<int> PositionId { get; set; }
        public string Task { get; set; }
        public string Description { get; set; }
    }
}
