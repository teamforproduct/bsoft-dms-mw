using System;
using BL.Model.DocumentCore.IncomingModel;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontDocumentSendList: ModifyDocumentSendList
    {
        /// <summary>
        /// ИД записи
        /// </summary>
        public int Id { get; set; }
        public bool IsInitial { get; set; }
        public int? StartEventId { get; set; }
        public int? CloseEventId { get; set; }

        public string SendTypeName { get; set; }
        public string SendTypeCode { get; set; }
        public bool SendTypeIsImportant { get; set; }

        public int? SourcePositionId { get; set; }
        public int SourceAgentId { get; set; }
        public int? TargetPositionId { get; set; }
        public int? TargetAgentId { get; set; }

        public string SourcePositionName { get; set; }
        public string SourcePositionExecutorAgentName { get; set; }
        public string SourceAgentName { get; set; }

        public string TargetPositionName { get; set; }
        public string TargetPositionExecutorAgentName { get; set; }
        public string TargetAgentName { get; set; }

        public string Task { get; set; }
        public string AccessLevelName { get; set; }

        public string GeneralInfo { get; set; }
    }
}
