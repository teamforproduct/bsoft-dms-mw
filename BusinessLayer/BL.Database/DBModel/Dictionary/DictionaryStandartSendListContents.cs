using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Database.DBModel.Admin;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionaryStandartSendListContents
    {
        public int Id { get; set; }
        public int StandartSendListId { get; set; }
        public int OrderNumber { get; set; }
        public int SendTypeId { get; set; }
        public Nullable<int> TargetPositionId { get; set; }
        public Nullable<int> TargetAgentId { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public int DueDay { get; set; }
        public Nullable<int> AccessLevelId { get; set; }
        public int LastChangeUserId { get; set; }
        public System.DateTime LastChangeDate { get; set; }

        public virtual DictionaryStandartSendLists StandartSendList { get; set; }
        public virtual DictionarySendTypes SendType { get; set; }
        public virtual DictionaryPositions TargetPosition { get; set; }
        public virtual DictionaryAgents TargetAgents { get; set; }
        public virtual AdminAccessLevels AccessLevel { get; set; }
    }
}
