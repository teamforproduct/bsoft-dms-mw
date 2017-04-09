using BL.Model.Common;
using BL.Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontDocumentEventAccess
    {
        public int? Id { get; set; }
        public int? ClientId { get; set; }
        public int? EntityTypeId { get; set; }
        public int? DocumentId { get; set; }
        public int? EventId { get; set; }
        public EnumEventAccessTypes? AccessType { get; set; }
        public Nullable<int> PositionId { get; set; }
        public string PositionName { get; set; }
        public Nullable<int> AgentId { get; set; }
        public string AgentName { get; set; }
        public int? PositionExecutorTypeId { get; set; }
        public string Name { get; set; }
        public Nullable<DateTime> SendDate { get; set; }
        public Nullable<DateTime> ReadDate { get; set; }
        public Nullable<int> ReadAgentId { get; set; }
        public bool? IsFavourite { get; set; }
        public bool? IsAddLater { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsNew { get; set; }
    }
}
