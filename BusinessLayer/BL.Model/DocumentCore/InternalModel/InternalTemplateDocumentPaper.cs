using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Common;
using BL.Model.DocumentCore.IncomingModel;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalTemplateDocumentPaper : LastChangeInfo
    {

        public InternalTemplateDocumentPaper()
        {
        }

        public InternalTemplateDocumentPaper(ModifyTemplateDocumentPaper model)
        {
            Id = model.Id;
            DocumentId = model.DocumentId;
            Name = model.Name;
            Description = model.Description;
            IsMain = model.IsMain;
            IsOriginal = model.IsOriginal;
            IsCopy = model.IsCopy;
            PageQuantity = model.PageQuantity;
        }

        public int Id { get; set; }
        public int DocumentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsMain { get; set; }
        public bool IsOriginal { get; set; }
        public bool IsCopy { get; set; }
        public int PageQuantity { get; set; }
        public int OrderNumber { get; set; }
    }
}
