using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Common;
using BL.Model.DocumentCore.IncomingModel;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalTemplateTask : LastChangeInfo
    {

        public InternalTemplateTask()
        {
        }

        public InternalTemplateTask(AddTemplateTask model)
        {
            SetInternalTemplateTask(model);
        }

        public InternalTemplateTask(ModifyTemplateTask model)
        {
            Id = model.Id;
            SetInternalTemplateTask(model);
        }

        private void SetInternalTemplateTask(AddTemplateTask model)
        {
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
