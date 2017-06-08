using System;
using BL.Model.Common;
using BL.Model.Enums;
using System.Collections.Generic;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontTemplateSendListAccessGroup
    {
        public int? Id { get; set; }
        public int? DocumentId { get; set; }
        public int? SendListId { get; set; }
        public EnumEventAccessTypes? AccessType { get; set; }   // получатель, копия, досылка
        public EnumEventAccessGroupTypes? AccessGroupType { get; set; } //тип группы, в т.ч. РГ по доку
        public int? CompanyId { get; set; }
        public int? DepartmentId { get; set; }
        public int? PositionId { get; set; }
        public int? AgentId { get; set; }
        public int? StandartSendListId { get; set; }
        public int? RecordId { get; set; }
        public string Name { get; set; }
        public List<string> Details { get; set; }

        public bool? IsActive { get; set; }
    }
}
