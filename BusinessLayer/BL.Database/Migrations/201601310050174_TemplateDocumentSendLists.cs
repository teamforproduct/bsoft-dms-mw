using System;
using System.Data.Entity.Migrations;

namespace BL.Database.Migrations
{
    public partial class TemplateDocumentSendLists : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TemplateDocumentRestrictedSendListsSet", "AgentId", "dbo.DictionaryAgents");
            DropIndex("dbo.TemplateDocumentRestrictedSendListsSet", new[] { "AgentId" });
            AddForeignKey("dbo.TemplateDocumentSendListsSet", "DocumentId", "dbo.AdminAccessLevels", "Id");
            AddForeignKey("dbo.TemplateDocumentSendListsSet", "DocumentId", "dbo.DictionarySendTypes", "Id");
            AddForeignKey("dbo.TemplateDocumentSendListsSet", "DocumentId", "dbo.DictionaryPositions", "Id");
            DropColumn("dbo.TemplateDocumentRestrictedSendListsSet", "AgentId");
            DropColumn("dbo.TemplateDocumentSendListsSet", "TargetAgentId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TemplateDocumentSendListsSet", "TargetAgentId", c => c.Int());
            AddColumn("dbo.TemplateDocumentRestrictedSendListsSet", "AgentId", c => c.Int());
            DropForeignKey("dbo.TemplateDocumentSendListsSet", "DocumentId", "dbo.DictionaryPositions");
            DropForeignKey("dbo.TemplateDocumentSendListsSet", "DocumentId", "dbo.DictionarySendTypes");
            DropForeignKey("dbo.TemplateDocumentSendListsSet", "DocumentId", "dbo.AdminAccessLevels");
            CreateIndex("dbo.TemplateDocumentRestrictedSendListsSet", "AgentId");
            AddForeignKey("dbo.TemplateDocumentRestrictedSendListsSet", "AgentId", "dbo.DictionaryAgents", "Id");
        }
    }
}
