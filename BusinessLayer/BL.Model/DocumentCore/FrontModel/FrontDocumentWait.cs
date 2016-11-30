﻿using BL.Model.Extensions;
using System;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontDocumentWait : FrontDocumentInfo
    {
        public int Id { get; set; }

        public int? ParentId { get; set; }
        public int OnEventId { get; set; }
        public int? OffEventId { get; set; }
        public int? ResultTypeId { get; set; }
        public string ResultTypeName { get; set; }

        private DateTime?  _DueDate; 
        public DateTime? DueDate { get { return _DueDate; } set { _DueDate=value.ToUTC(); } }
		
        private DateTime?  _AttentionDate; 
        public DateTime? AttentionDate { get { return _AttentionDate; } set { _AttentionDate=value.ToUTC(); } }
		
        public string TargetDescription { get; set; }
        
		//private DateTime?  _TargetAttentionDate; 
        //public DateTime? TargetAttentionDate { get { return _TargetAttentionDate; } set { _TargetAttentionDate=value.ToUTC(); } }
		
        public FrontDocumentEvent OnEvent { get; set; }
        public FrontDocumentEvent OffEvent { get; set; }
        public bool IsClosed { get; set; }
        public bool IsOverDue { get; set; }
        public string SourcePositionExecutorAgentName { get; set; }
        public string TargetPositionExecutorAgentName { get; set; }
        public int? RecordCount { get; set; }



    }
}
