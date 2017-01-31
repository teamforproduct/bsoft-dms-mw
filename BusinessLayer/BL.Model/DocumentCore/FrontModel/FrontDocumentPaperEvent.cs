﻿using BL.Model.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontDocumentPaperEvent : FrontDocumentInfo
    {
        public int Id { get; set; }

        public int? EventType { get; set; }
        public string EventTypeName { get; set; }

        public DateTime? PlanDate { get { return _PlanDate; } set { _PlanDate = value.ToUTC(); } }
        private DateTime? _PlanDate;

        public DateTime? SendDate { get { return _SendDate; } set { _SendDate = value.ToUTC(); } }
        private DateTime? _SendDate;

        public DateTime? RecieveDate { get { return _RecieveDate; } set { _RecieveDate = value.ToUTC(); } }
        private DateTime? _RecieveDate;


        public string Description { get; set; }






        public string SourceAgentName { get; set; }
        public string SourcePositionExecutorAgentName { get; set; }
        public string SourcePositionName { get; set; }

        public string TargetAgentName { get; set; }
        public string TargetPositionExecutorAgentName { get; set; }
        public string TargetPositionName { get; set; }

    }
}
