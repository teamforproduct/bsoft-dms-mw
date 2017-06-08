using System;
using BL.Model.Extensions;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.FrontModel
{
    /// <summary>
    /// Describe the display model for the Document events.
    /// </summary>
    public class FrontDocumentEvent : FrontDocumentInfo
    {
        public int Id { get; set; }

        public EnumEventTypes? EventType { get; set; }
        public string EventTypeName { get; set; }

        public DateTime? Date { get { return _date; } set { _date = value.ToUTC(); } }
        private DateTime? _date;

        public DateTime? CreateDate { get { return _createDate; } set { _createDate = value.ToUTC(); } }
        private DateTime? _createDate;


        public string Task { get; set; }
        public string Description { get; set; }
        public string AddDescription { get; set; }
        
        public DateTime? DueDate { get { return _DueDate; } set { _DueDate=value.ToUTC(); } }
        private DateTime?  _DueDate; 
        
        public DateTime? CloseDate { get { return _CloseDate; } set { _CloseDate=value.ToUTC(); } }
        private DateTime?  _CloseDate; 
		
        public bool? IsOnEvent { get; set; }

        
        public DateTime? LastChangeDate { get { return _LastChangeDate; } set { _LastChangeDate=value.ToUTC(); } }
        private DateTime?  _LastChangeDate; 
        
        public DateTime? ReadDate { get { return _ReadDate; } set { _ReadDate=value.ToUTC(); } }
        private DateTime?  _ReadDate;
        public int? ReadAgentId { get; set; }
        public string ReadAgentName { get; set; }
        public bool? IsRead { get; set; }
        public int? SourceAgentId { get; set; }
        public string SourceAgentName { get; set; }
        public int? SourcePositionExecutorAgentId { get; set; }
        public string SourcePositionExecutorAgentName { get; set; }
        public int? TargetPositionId { get; set; }
        public int? TargetAgentId { get; set; }
        public string TargetAgentName { get; set; }
        public int? TargetPositionExecutorAgentId { get; set; }
        public string TargetPositionExecutorAgentName { get; set; }
        public int? SourcePositionId { get; set; }
        public string SourcePositionName { get; set; }
        public string TargetPositionName { get; set; }
        public int? PaperId { get; set; }
        public string PaperName { get; set; }
        public bool? PaperIsMain { get; set; }
        public bool? PaperIsOriginal { get; set; }
        public bool? PaperIsCopy { get; set; }
        public int? PaperOrderNumber { get; set; }
        
        public DateTime? PaperPlanDate { get { return _PaperPlanDate; } set { _PaperPlanDate=value.ToUTC(); } }
        private DateTime?  _PaperPlanDate; 
        
        public DateTime? PaperSendDate { get { return _PaperSendDate; } set { _PaperSendDate=value.ToUTC(); } }
        private DateTime?  _PaperSendDate; 
        
        public DateTime? PaperRecieveDate { get { return _PaperRecieveDate; } set { _PaperRecieveDate=value.ToUTC(); } }
        private DateTime?  _PaperRecieveDate;
        [XmlIgnore]
        [IgnoreDataMember]
        public FrontDocumentWait OnWait { get; set; }
        public string PaperPlanAgentName { get; set; }
        public string PaperSendAgentName { get; set; }
        public string PaperRecieveAgentName { get; set; }
        public IEnumerable<FrontDocumentEventAccess> Accesses { get; set; }
        public IEnumerable<FrontDocumentEventAccessGroup> AccessGroups { get; set; }

        public IEnumerable<FrontDocumentFile> Files { get; set; }

    }
}
