using System;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontTemplateDocumentRestrictedSendLists
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int? PositionId { get; set; }
        public int AccessLevelId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        public string PositionName { get; set; }
        public string AccessLevelName { get; set; }

        public string GeneralInfo { get; set; }
    }
}
