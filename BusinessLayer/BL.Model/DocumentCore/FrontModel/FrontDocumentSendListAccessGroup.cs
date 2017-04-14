using System;
using BL.Model.Common;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontDocumentSendListAccessGroup
    {
        public int? Id { get; set; }
        public int? ClientId { get; set; }
        public int? EntityTypeId { get; set; }
        public int? DocumentId { get; set; }
        public int? SendListId { get; set; }
        public EnumEventAccessTypes? AccessType { get; set; }   // получатель, копия, досылка
        public EnumEventAccessGroupTypes? AccessGroupType { get; set; } //тип группы, в т.ч. РГ по доку
        public int? CompanyId { get; set; }
        public int? DepartmentId { get; set; }
        public int? PositionId { get; set; }
        public int? AgentId { get; set; }
        public int? StandartSendListId { get; set; }
        public string Name { get; set; }

        public bool? IsActive { get; set; }
    }
}
