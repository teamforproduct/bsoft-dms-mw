﻿using System;

namespace BL.Model.DocumentCore
{
    public class BaseDocumentRestrictedSendList : ModifyDocumentRestrictedSendList
    {
        public int Id { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        public string PositionName { get; set; }
        public string PositionExecutorAgentName { get; set; }
        public string AccessLevelName { get; set; }

        public string GeneralInfo { get; set; }
    }
}
