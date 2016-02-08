using System;
using BL.Model.Enums;

namespace BL.Model.DocumentCore
{
    public class BaseTemplateDocumentSendLists
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public EnumSendType SendType { get; set; }
        public Nullable<int> TargetPositionId { get; set; }
        public string Description { get; set; }
        public Nullable<DateTime> DueDate { get; set; }
        public int Stage { get; set; }
        public int? DueDay { get; set; }
        public EnumDocumentAccess AccessLevel { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        public string PositionName { get; set; }
        public string AccessLevelName { get; set; }

        public string GeneralInfo { get; set; }

    }
}
