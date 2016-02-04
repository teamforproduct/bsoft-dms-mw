﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore
{
    public class BaseDocumentSendList: ModifyDocumentSendList
    {
        public int Id { get; set; }
        public bool IsInitial { get; set; }
        public Nullable<int> EventId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        public string SendTypeName { get; set; }
        public string SendTypeCode { get; set; }
        public bool SendTypeIsImpotant { get; set; }
        public string TargetPositionName { get; set; }
        public string TargetPositionExecutorAgentName { get; set; }
        public string AccessLevelName { get; set; }

        public string GeneralInfo { get; set; }
    }
}
