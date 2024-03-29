﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Database.DBModel.Dictionary;

namespace BL.Database.DBModel.Document
{
    public class DocumentSendListAccessGroups
    {
        public int Id { get; set; }
        [Index("IX_ClientId", 1)]
        public int ClientId { get; set; }
        [Index("IX_EntityTypeId", 1)]
        public int EntityTypeId { get; set; }
        public int DocumentId { get; set; }
        public int SendListId { get; set; }
        public int AccessTypeId { get; set; }   // получатель, копия, досылка
        public int AccessGroupTypeId { get; set; } //тип группы, в т.ч. РГ по доку
        public int? CompanyId { get; set; }
        public int? DepartmentId { get; set; }
        public int? PositionId { get; set; }
        public int? AgentId { get; set; }
        public int? StandartSendListId { get; set; }


        public bool IsActive { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        [ForeignKey("DocumentId")]
        public virtual Documents Document { get; set; }
        [ForeignKey("SendListId")]
        public virtual DocumentSendLists SendList { get; set; }
        [ForeignKey("CompanyId")]
        public virtual DictionaryCompanies Company { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual DictionaryDepartments Department { get; set; }
        [ForeignKey("PositionId")]
        public virtual DictionaryPositions Position { get; set; }
        [ForeignKey("AgentId")]
        public virtual DictionaryAgents Agent { get; set; }
        [ForeignKey("StandartSendListId")]
        public virtual DictionaryStandartSendLists StandartSendList { get; set; }




    }
}
