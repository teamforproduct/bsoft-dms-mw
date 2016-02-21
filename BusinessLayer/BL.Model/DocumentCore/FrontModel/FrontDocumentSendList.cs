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
