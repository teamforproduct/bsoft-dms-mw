using BL.Model.Common;
using BL.Model.Enums;
using System;

namespace BL.Model.DictionaryCore.InternalModel
{
    public class InternalDictionaryStandartSendListContent: LastChangeInfo
    {
        public int Id { get; set; }
        public int StandartSendListId { get; set; }
        public int Stage { get; set; }
        public EnumSendTypes SendType { get; set; }
        public int? TargetPositionId { get; set; }
        public string Task { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        public int? DueDay { get; set; }
        public EnumDocumentAccesses AccessLevel { get; set; }
    }
}
