using BL.Model.DocumentCore.Actions;
using System;

namespace BL.Model.DocumentCore
{
    public class ВaseDocumentLink: AddDocumentLink
    {
        public ВaseDocumentLink()
        {
        }

        public ВaseDocumentLink(AddDocumentLink model) : this()
        {
            DocumentId = model.DocumentId;
            ParentDocumentId = model.ParentDocumentId;
            LinkTypeId = model.LinkTypeId;
        }
        public int Id { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        public string LinkTypeName { get; set; }
        public FullDocument Document { get; set; }
        public FullDocument ParentDocument { get; set; }
        public string GeneralInfo { get; set; }
    }
}
