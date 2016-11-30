using System;
using BL.Model.Enums;
using BL.Model.Extensions;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontTemplateDocumentSendList
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int? SendType { get; set; }
        public string SendTypeName { get; set; }

        public int? TargetPositionId { get; set; }
        public string TargetPositionName { get; set; }
        public string Description { get; set; }
        public int Stage { get; set; }
        public int? TaskId { get; set; }
        public string Task { get; set; }
        public int? DueDay { get; set; }
        public int? AccessLevelId { get; set; }
        public string AccessLevelName { get; set; }
        public bool? IsWorkGroup { get; set; }
        public bool? IsAddControl { get; set; }
		
        private DateTime?  _SelfDueDate; 
        public DateTime? SelfDueDate { get { return _SelfDueDate; } set { _SelfDueDate=value.ToUTC(); } }
		
        public int? SelfDueDay { get; set; }
		
        private DateTime?  _SelfAttentionDate; 
        public DateTime? SelfAttentionDate { get { return _SelfAttentionDate; } set { _SelfAttentionDate=value.ToUTC(); } }
		
        public bool? IsAvailableWithinTask { get; set; }

    }
}
