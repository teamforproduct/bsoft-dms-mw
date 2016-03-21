using System;
using System.Data.Entity.Migrations;

namespace BL.Database.Migrations
{
    public partial class RestrictedSendLists : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Documents", "RestrictedSendListId", "dbo.DictionaryStandartSendLists");
            DropForeignKey("dbo.TemplateDocuments", "RestrictedSendListId", "dbo.DictionaryStandartSendLists");
            DropIndex("dbo.Documents", new[] { "RestrictedSendListId" });
            DropIndex("dbo.TemplateDocuments", new[] { "RestrictedSendListId" });
            CreateTable(
                "dbo.DocumentRestrictedSendLists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        PositionId = c.Int(),
                        AgentId = c.Int(),
                        AccessLevelId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AdminAccessLevels", t => t.AccessLevelId)
                .ForeignKey("dbo.DictionaryAgents", t => t.AgentId)
                .ForeignKey("dbo.Documents", t => t.DocumentId)
                .ForeignKey("dbo.DictionaryPositions", t => t.PositionId)
                .Index(t => t.DocumentId)
                .Index(t => t.PositionId)
                .Index(t => t.AgentId)
                .Index(t => t.AccessLevelId);
            
            CreateTable(
                "dbo.TemplateDocumentRestrictedSendListsSet",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        PositionId = c.Int(),
                        AgentId = c.Int(),
                        AccessLevelId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AdminAccessLevels", t => t.AccessLevelId)
                .ForeignKey("dbo.DictionaryAgents", t => t.AgentId)
                .ForeignKey("dbo.TemplateDocuments", t => t.DocumentId)
                .ForeignKey("dbo.DictionaryPositions", t => t.PositionId)
                .Index(t => t.DocumentId)
                .Index(t => t.PositionId)
                .Index(t => t.AgentId)
                .Index(t => t.AccessLevelId);
            
            DropColumn("dbo.Documents", "RestrictedSendListId");
            DropColumn("dbo.TemplateDocuments", "RestrictedSendListId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TemplateDocuments", "RestrictedSendListId", c => c.Int());
            AddColumn("dbo.Documents", "RestrictedSendListId", c => c.Int());
            DropForeignKey("dbo.TemplateDocumentRestrictedSendListsSet", "PositionId", "dbo.DictionaryPositions");
            DropForeignKey("dbo.TemplateDocumentRestrictedSendListsSet", "DocumentId", "dbo.TemplateDocuments");
            DropForeignKey("dbo.TemplateDocumentRestrictedSendListsSet", "AgentId", "dbo.DictionaryAgents");
            DropForeignKey("dbo.TemplateDocumentRestrictedSendListsSet", "AccessLevelId", "dbo.AdminAccessLevels");
            DropForeignKey("dbo.DocumentRestrictedSendLists", "PositionId", "dbo.DictionaryPositions");
            DropForeignKey("dbo.DocumentRestrictedSendLists", "DocumentId", "dbo.Documents");
            DropForeignKey("dbo.DocumentRestrictedSendLists", "AgentId", "dbo.DictionaryAgents");
            DropForeignKey("dbo.DocumentRestrictedSendLists", "AccessLevelId", "dbo.AdminAccessLevels");
            DropIndex("dbo.TemplateDocumentRestrictedSendListsSet", new[] { "AccessLevelId" });
            DropIndex("dbo.TemplateDocumentRestrictedSendListsSet", new[] { "AgentId" });
            DropIndex("dbo.TemplateDocumentRestrictedSendListsSet", new[] { "PositionId" });
            DropIndex("dbo.TemplateDocumentRestrictedSendListsSet", new[] { "DocumentId" });
            DropIndex("dbo.DocumentRestrictedSendLists", new[] { "AccessLevelId" });
            DropIndex("dbo.DocumentRestrictedSendLists", new[] { "AgentId" });
            DropIndex("dbo.DocumentRestrictedSendLists", new[] { "PositionId" });
            DropIndex("dbo.DocumentRestrictedSendLists", new[] { "DocumentId" });
            DropTable("dbo.TemplateDocumentRestrictedSendListsSet");
            DropTable("dbo.DocumentRestrictedSendLists");
            CreateIndex("dbo.TemplateDocuments", "RestrictedSendListId");
            CreateIndex("dbo.Documents", "RestrictedSendListId");
            AddForeignKey("dbo.TemplateDocuments", "RestrictedSendListId", "dbo.DictionaryStandartSendLists", "Id");
            AddForeignKey("dbo.Documents", "RestrictedSendListId", "dbo.DictionaryStandartSendLists", "Id");
        }
    }
}
