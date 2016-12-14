using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Common;
using BL.Model.DocumentCore.IncomingModel;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalTemplateDocumentTask : LastChangeInfo
    {

        public InternalTemplateDocumentTask()
        {
        }

        public InternalTemplateDocumentTask(ModifyTemplateDocumentTask model)
        {
            Id = model.Id;
            DocumentId = model.DocumentId;
            Task = model.Task;
            Description = model.Description;
        }

        public int Id { get; set; }
        public int DocumentId { get; set; }
        public string Task { get; set; }
        public string Description { get; set; }
    }
}
