using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Dictionary
{
    public partial class DictionaryAgentFavorites
    {
        public int Id { get; set; }

        [Index("IX_AgentObjectModuleFeature", 1, IsUnique = true)]
        public int AgentId { get; set; }

        [Index("IX_AgentObjectModuleFeature", 2, IsUnique = true)]
        public int ObjectId { get; set; }

        [Index("IX_AgentObjectModuleFeature", 3, IsUnique = true)]
        [MaxLength(200)]
        public string Module { get; set; }

        [Index("IX_AgentObjectModuleFeature", 4, IsUnique = true)]
        [MaxLength(200)]
        public string Feature { get; set; }

        public DateTime Date { get; set; }

        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }



        [ForeignKey("AgentId")]
        public virtual DictionaryAgents Agent { get; set; }
    }
}
