using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionaryAgentContacts
    {
        public int Id { get; set; }
        [Index("IX_AgentContactTypeContact", 1, IsUnique = true)]
        public int AgentId { get; set; }
        [Index("IX_AgentContactTypeContact", 2, IsUnique = true)]
        [Index("IX_ContactTypeId", 1)]
        public int ContactTypeId { get; set; }
        [MaxLength(400)]
        [Index("IX_AgentContactTypeContact", 3, IsUnique = true)]
        public string Contact { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("AgentId")]
        public virtual DictionaryAgents Agent { get; set; }
        [ForeignKey("ContactTypeId")]
        public virtual DictionaryContactTypes ContactType { get; set; }
    }
}
