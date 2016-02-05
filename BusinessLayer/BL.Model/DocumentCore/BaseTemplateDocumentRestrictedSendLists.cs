using System;
using BL.Model.Enums;

namespace BL.Model.DocumentCore
{
    public class BaseTemplateDocumentRestrictedSendLists
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public Nullable<int> PositionId { get; set; }
        public EnumDocumentAccess AccessLevel { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        public string PositionName { get; set; }
        public string AccessLevelName { get; set; }

        public string GeneralInfo { get; set; }
    }
}
