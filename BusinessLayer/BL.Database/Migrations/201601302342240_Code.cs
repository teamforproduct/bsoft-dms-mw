using System;
using System.Data.Entity.Migrations;

namespace BL.Database.Migrations
{
   
    public partial class Code : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DictionaryAgentPersons",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AgentId = c.Int(nullable: false),
                        Name = c.String(),
                        PersonAgentId = c.Int(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                        DictionaryAgents_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DictionaryAgents", t => t.AgentId)
                .ForeignKey("dbo.DictionaryAgents", t => t.PersonAgentId)
                .ForeignKey("dbo.DictionaryAgents", t => t.DictionaryAgents_Id)
                .Index(t => t.AgentId)
                .Index(t => t.PersonAgentId)
                .Index(t => t.DictionaryAgents_Id);
            
            AddColumn("dbo.DictionaryAgents", "IsIndividual", c => c.Boolean(nullable: false));
            AddColumn("dbo.DictionaryAgents", "IsEmployee", c => c.Boolean(nullable: false));
            AddColumn("dbo.DictionarySendTypes", "Code", c => c.String());
            AddColumn("dbo.DictionarySubordinationTypes", "Name", c => c.String());
            AddColumn("dbo.DictionaryEventTypes", "Code", c => c.String());
            AddColumn("dbo.DictionaryImpotanceEventTypes", "Code", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DictionaryAgentPersons", "DictionaryAgents_Id", "dbo.DictionaryAgents");
            DropForeignKey("dbo.DictionaryAgentPersons", "PersonAgentId", "dbo.DictionaryAgents");
            DropForeignKey("dbo.DictionaryAgentPersons", "AgentId", "dbo.DictionaryAgents");
            DropIndex("dbo.DictionaryAgentPersons", new[] { "DictionaryAgents_Id" });
            DropIndex("dbo.DictionaryAgentPersons", new[] { "PersonAgentId" });
            DropIndex("dbo.DictionaryAgentPersons", new[] { "AgentId" });
            DropColumn("dbo.DictionaryImpotanceEventTypes", "Code");
            DropColumn("dbo.DictionaryEventTypes", "Code");
            DropColumn("dbo.DictionarySubordinationTypes", "Name");
            DropColumn("dbo.DictionarySendTypes", "Code");
            DropColumn("dbo.DictionaryAgents", "IsEmployee");
            DropColumn("dbo.DictionaryAgents", "IsIndividual");
            DropTable("dbo.DictionaryAgentPersons");
        }
    }
}
