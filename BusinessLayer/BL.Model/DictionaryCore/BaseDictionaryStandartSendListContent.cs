using System;

namespace BL.Model.DictionaryCore
{
    public class BaseDictionaryStandartSendListContent
    {
        public int Id { get; set; }
        public int StandartSendListId { get; set; }
        public int Stage { get; set; }
        public int SendTypeId { get; set; }
        public int? TargetPositionId { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        public int? DueDay { get; set; }
        public int? AccessLevelId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        public string SendTypeName { get; set; }
        public string TargetPositionName { get; set; }
        public string TargetPositionExecutorAgentName { get; set; }
        public string AccessLevelName { get; set; }

    }
}
