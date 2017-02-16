using BL.Database.DBModel.Dictionary;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.System
{
    public class SystemSearchQueryLogs
    {
        public int Id { get; set; }
        [Index("IX_ClientId", 1)]
        public int ClientId { get; set; }
        public int ModuleId { get; set; }
        public int FeatureId { get; set; }
        [MaxLength(2000)]
        public string SearchQueryText { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

    }
}