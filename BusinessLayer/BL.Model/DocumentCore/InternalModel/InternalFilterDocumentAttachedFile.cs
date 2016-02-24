using System;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.IncomingModel;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalFilterDocumentAttachedFile : FilterDocumentFileIdentity //TODO : LastChangeInfo?
    {
        public int Id { get; set; }
        public byte[] FileContent { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string FileType { get; set; }
        public bool IsAdditional { get; set; }
        public DateTime Date { get; set; }
        public string Hash { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        public bool WasChangedExternal { get; set; }
    }
}