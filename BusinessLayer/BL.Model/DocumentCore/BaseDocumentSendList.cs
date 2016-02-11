using System;

namespace BL.Model.DocumentCore
{
    public class BaseDocumentSendList: ModifyDocumentSendList
    {
        public int Id { get; set; }
        public bool IsInitial { get; set; }
        public Nullable<int> StartEventId { get; set; }
        public Nullable<int> CloseEventId { get; set; }
        public int Stage { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        public string SendTypeName { get; set; }
        public string SendTypeCode { get; set; }
        public bool SendTypeIsImportant { get; set; }
        public string TargetPositionName { get; set; }
        public string TargetPositionExecutorAgentName { get; set; }
        public string AccessLevelName { get; set; }

        public string GeneralInfo { get; set; }
    }
}
