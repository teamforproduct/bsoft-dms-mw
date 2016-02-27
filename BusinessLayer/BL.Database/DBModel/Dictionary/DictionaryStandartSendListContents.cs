using System;
using BL.Database.DBModel.Admin;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionaryStandartSendListContents
    {
        public int Id { get; set; }
        public int StandartSendListId { get; set; }
        public int Stage { get; set; }
        public int SendTypeId { get; set; }
        public Nullable<int> TargetPositionId { get; set; }
        public Nullable<int> TargetAgentId { get; set; }
        public string Task { get; set; }
        public string Description { get; set; }
        public Nullable<DateTime> DueDate { get; set; }
        public Nullable<int> DueDay { get; set; }
        public Nullable<int> AccessLevelId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("StandartSendListId")]
        public virtual DictionaryStandartSendLists StandartSendList { get; set; }
        [ForeignKey("SendTypeId")]
        public virtual DictionarySendTypes SendType { get; set; }
        [ForeignKey("TargetPositionId")]
        public virtual DictionaryPositions TargetPosition { get; set; }
        [ForeignKey("TargetAgentId")]
        public virtual DictionaryAgents TargetAgent { get; set; }
        [ForeignKey("AccessLevelId")]
        public virtual AdminAccessLevels AccessLevel { get; set; }
    }
}
