namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class agent1to2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DictionaryAgentPersons", "AgentId", "dbo.DictionaryAgents");
            DropForeignKey("dbo.DictionaryAgentPersons", "PersonAgentId", "dbo.DictionaryAgents");
            DropForeignKey("dbo.Documents", "SenderAgentPersonId", "dbo.DictionaryAgentPersons");
            DropForeignKey("dbo.TemplateDocuments", "SenderAgentPersonId", "dbo.DictionaryAgentPersons");
            DropIndex("dbo.DictionaryAgentPersons", new[] { "AgentId" });
            DropIndex("dbo.DictionaryAgentPersons", new[] { "PersonAgentId" });
            DropIndex("dbo.Documents", new[] { "SenderAgentPersonId" });
            DropIndex("dbo.TemplateDocuments", new[] { "SenderAgentPersonId" });
            DropColumn("dbo.DictionaryAgents", "TaxCode");
            DropColumn("dbo.DictionaryAgentPersons", "AgentId");
            DropColumn("dbo.DictionaryAgentPersons", "PersonAgentId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DictionaryAgentPersons", "PersonAgentId", c => c.Int());
            AddColumn("dbo.DictionaryAgentPersons", "AgentId", c => c.Int(nullable: false));
            AddColumn("dbo.DictionaryAgents", "TaxCode", c => c.String());
            CreateIndex("dbo.TemplateDocuments", "SenderAgentPersonId");
            CreateIndex("dbo.Documents", "SenderAgentPersonId");
            CreateIndex("dbo.DictionaryAgentPersons", "PersonAgentId");
            CreateIndex("dbo.DictionaryAgentPersons", "AgentId");
            AddForeignKey("dbo.TemplateDocuments", "SenderAgentPersonId", "dbo.DictionaryAgentPersons", "Id");
            AddForeignKey("dbo.Documents", "SenderAgentPersonId", "dbo.DictionaryAgentPersons", "Id");
            AddForeignKey("dbo.DictionaryAgentPersons", "PersonAgentId", "dbo.DictionaryAgents", "Id");
            AddForeignKey("dbo.DictionaryAgentPersons", "AgentId", "dbo.DictionaryAgents", "Id");
        }
    }
}
