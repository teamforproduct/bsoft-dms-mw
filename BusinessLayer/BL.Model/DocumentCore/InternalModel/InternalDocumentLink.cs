
using System;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalDocumentLink 
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int EntityTypeId { get; set; }
        public int LinkTypeId { get; set; }
        public int DocumentId { get; set; }
        public int ParentDocumentId { get; set; }
        public int? ExecutorPositionId { get; set; }

    }
}
