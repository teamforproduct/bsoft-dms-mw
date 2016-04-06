using System;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontTemplateDocumentSendLists
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public EnumSendTypes SendType { get; set; }
        
        public Nullable<int> TargetPositionId { get; set; }
        public string Description { get; set; }
        public int Stage { get; set; }
        public string Task { get; set; }
        public int? DueDay { get; set; }
        public int AccessLevelId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        public bool IsAddControl { get; set; }
        public bool IsAvailableWithinTask { get; set; }


        public string PositionName { get; set; }
        public string SendTypeName { get; set; }
        public string AccessLevelName { get; set; }

        public string GeneralInfo { get; set; }

    }
}
