using System;
using System.Data.Entity.Migrations;

namespace BL.Database.Migrations
{
    public partial class TemplateDocumentSendLists : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TemplateDocumentRestrictedSendLists", "AgentId", "dbo.DictionaryAgents");
            DropIndex("dbo.TemplateDocumentRestrictedSendLists", new[] { "AgentId" });
            AddForeignKey("dbo.TemplateDocumentSendLists", "DocumentId", "dbo.AdminAccessLevels", "Id");
            AddForeignKey("dbo.TemplateDocumentSendLists", "DocumentId", "dbo.DictionarySendTypes", "Id");
            AddForeignKey("dbo.TemplateDocumentSendLists", "DocumentId", "dbo.DictionaryPositions", "Id");
            DropColumn("dbo.TemplateDocumentRestrictedSendLists", "AgentId");
            DropColumn("dbo.TemplateDocumentSendLists", "TargetAgentId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TemplateDocumentSendLists", "TargetAgentId", c => c.Int());
            AddColumn("dbo.TemplateDocumentRestrictedSendLists", "AgentId", c => c.Int());
            DropForeignKey("dbo.TemplateDocumentSendLists", "DocumentId", "dbo.DictionaryPositions");
            DropForeignKey("dbo.TemplateDocumentSendLists", "DocumentId", "dbo.DictionarySendTypes");
            DropForeignKey("dbo.TemplateDocumentSendLists", "DocumentId", "dbo.AdminAccessLevels");
            CreateIndex("dbo.TemplateDocumentRestrictedSendLists", "AgentId");
            AddForeignKey("dbo.TemplateDocumentRestrictedSendLists", "AgentId", "dbo.DictionaryAgents", "Id");
        }
    }
}
