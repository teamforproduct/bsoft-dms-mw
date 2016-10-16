using BL.Database.DBModel.Admin;
using BL.Database.DBModel.Encryption;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionaryAgents
    {
        public DictionaryAgents()
        {
       //     this.Positions = new HashSet<DictionaryPositions>();
            this.AgentAddresses = new HashSet<DictionaryAgentAddresses>();
            this.AgentContacts = new HashSet<DictionaryAgentContacts>();
            this.AgentAccounts = new HashSet<DictionaryAgentAccounts>();
            this.Certificates = new HashSet<EncryptionCertificates>();

        }

        public int Id { get; set; }
        [Index("IX_Name", 2, IsUnique = true)]
        [Index("IX_ClientId", 1)]
        public int ClientId { get; set; }
        [Index("IX_Name", 1, IsUnique = true)]
        [MaxLength(400)]
        public string Name { get; set; }
        public Nullable<int> ResidentTypeId { get; set; }
        //public bool IsCompany { get; set; }
        //public bool IsIndividual { get; set; }
        //public bool IsEmployee { get; set; }
        //public bool IsBank { get; set; }
        //public bool IsUser { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        public bool IsActive { get; set; }

        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

     //   public virtual ICollection<DictionaryPositions> Positions { get; set; }

        [ForeignKey("Id")]
        public virtual DictionaryAgentCompanies AgentCompany { get; set; }
        [ForeignKey("Id")]
        public virtual DictionaryAgentPersons AgentPerson { get; set; }
        [ForeignKey("Id")]
        public virtual DictionaryAgentBanks AgentBank { get; set; }
        [ForeignKey("Id")]
        public virtual DictionaryAgentEmployees AgentEmployee { get; set; }

        [ForeignKey("Id")]
        public virtual DictionaryAgentUsers AgentUser { get; set; }
        [ForeignKey("Id")]
        public virtual DictionaryCompanies Company { get; set; }

        public virtual DictionaryResidentTypes ResidentType { get; set; }

        public virtual ICollection<DictionaryAgentAddresses> AgentAddresses { get; set; }

        public virtual ICollection<DictionaryAgentContacts> AgentContacts { get; set; }

        public virtual ICollection<DictionaryAgentAccounts> AgentAccounts { get; set; }

        public virtual ICollection<EncryptionCertificates> Certificates { get; set; }

        //[ForeignKey("AgentId")]
        //public virtual ICollection<DictionaryAgentPersons> AgentPersonsAgents { get; set; }

        //[ForeignKey("PersonAgentId")]
        //public virtual ICollection<DictionaryAgentPersons> AgentPersonsPersonAgents { get; set; }

        [ForeignKey("UserId")]
        public virtual ICollection<AdminUserRoles> UserRoles { get; set; }
    }
}
