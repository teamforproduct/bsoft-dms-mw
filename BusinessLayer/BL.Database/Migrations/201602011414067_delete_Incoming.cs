using System;
using System.Data.Entity.Migrations;

namespace BL.Database.Migrations
{
    public partial class delete_Incoming : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DocumentIncomingDetails", "SenderAgentId", "dbo.DictionaryAgents");
            DropForeignKey("dbo.DocumentIncomingDetails", "Id", "dbo.Documents");
            DropForeignKey("dbo.TemplateDocumentIncomingDetails", "SenderAgentId", "dbo.DictionaryAgents");
            DropForeignKey("dbo.TemplateDocumentIncomingDetails", "Id", "dbo.TemplateDocuments");
            DropIndex("dbo.DictionaryStandartSendLists", new[] { "PositionId" });
            DropIndex("dbo.DocumentIncomingDetails", new[] { "Id" });
            DropIndex("dbo.DocumentIncomingDetails", new[] { "SenderAgentId" });
            DropIndex("dbo.TemplateDocumentIncomingDetails", new[] { "Id" });
            DropIndex("dbo.TemplateDocumentIncomingDetails", new[] { "SenderAgentId" });
            AddColumn("dbo.Documents", "SenderAgentId", c => c.Int());
            AddColumn("dbo.Documents", "SenderAgentPersonId", c => c.Int());
            AddColumn("dbo.Documents", "SenderNumber", c => c.String());
            AddColumn("dbo.Documents", "SenderDate", c => c.DateTime());
            AddColumn("dbo.Documents", "Addressee", c => c.String());
            AddColumn("dbo.TemplateDocuments", "SenderAgentId", c => c.Int());
            AddColumn("dbo.TemplateDocuments", "SenderAgentPersonId", c => c.Int());
            AddColumn("dbo.TemplateDocuments", "SenderNumber", c => c.String());
            AddColumn("dbo.TemplateDocuments", "SenderDate", c => c.DateTime());
            AddColumn("dbo.TemplateDocuments", "Addressee", c => c.String());
            AlterColumn("dbo.DictionaryStandartSendLists", "PositionId", c => c.Int());
            AlterColumn("dbo.DictionaryStandartSendListContents", "DueDay", c => c.Int());
            CreateIndex("dbo.DictionaryStandartSendLists", "PositionId");
            CreateIndex("dbo.Documents", "SenderAgentId");
            CreateIndex("dbo.Documents", "SenderAgentPersonId");
            CreateIndex("dbo.TemplateDocuments", "SenderAgentId");
            CreateIndex("dbo.TemplateDocuments", "SenderAgentPersonId");
            AddForeignKey("dbo.Documents", "SenderAgentId", "dbo.DictionaryAgents", "Id");
            AddForeignKey("dbo.Documents", "SenderAgentPersonId", "dbo.DictionaryAgentPersons", "Id");
            AddForeignKey("dbo.TemplateDocuments", "SenderAgentId", "dbo.DictionaryAgents", "Id");
            AddForeignKey("dbo.TemplateDocuments", "SenderAgentPersonId", "dbo.DictionaryAgentPersons", "Id");
            DropTable("dbo.DocumentIncomingDetails");
            DropTable("dbo.TemplateDocumentIncomingDetails");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.TemplateDocumentIncomingDetails",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        SenderAgentId = c.Int(),
                        SenderPerson = c.String(),
                        SenderNumber = c.String(),
                        SenderDate = c.DateTime(),
                        Addressee = c.String(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DocumentIncomingDetails",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        SenderAgentId = c.Int(nullable: false),
                        SenderPerson = c.String(),
                        SenderNumber = c.String(),
                        SenderDate = c.DateTime(nullable: false),
                        Addressee = c.String(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.TemplateDocuments", "SenderAgentPersonId", "dbo.DictionaryAgentPersons");
            DropForeignKey("dbo.TemplateDocuments", "SenderAgentId", "dbo.DictionaryAgents");
            DropForeignKey("dbo.Documents", "SenderAgentPersonId", "dbo.DictionaryAgentPersons");
            DropForeignKey("dbo.Documents", "SenderAgentId", "dbo.DictionaryAgents");
            DropIndex("dbo.TemplateDocuments", new[] { "SenderAgentPersonId" });
            DropIndex("dbo.TemplateDocuments", new[] { "SenderAgentId" });
            DropIndex("dbo.Documents", new[] { "SenderAgentPersonId" });
            DropIndex("dbo.Documents", new[] { "SenderAgentId" });
            DropIndex("dbo.DictionaryStandartSendLists", new[] { "PositionId" });
            AlterColumn("dbo.DictionaryStandartSendListContents", "DueDay", c => c.Int(nullable: false));
            AlterColumn("dbo.DictionaryStandartSendLists", "PositionId", c => c.Int(nullable: false));
            DropColumn("dbo.TemplateDocuments", "Addressee");
            DropColumn("dbo.TemplateDocuments", "SenderDate");
            DropColumn("dbo.TemplateDocuments", "SenderNumber");
            DropColumn("dbo.TemplateDocuments", "SenderAgentPersonId");
            DropColumn("dbo.TemplateDocuments", "SenderAgentId");
            DropColumn("dbo.Documents", "Addressee");
            DropColumn("dbo.Documents", "SenderDate");
            DropColumn("dbo.Documents", "SenderNumber");
            DropColumn("dbo.Documents", "SenderAgentPersonId");
            DropColumn("dbo.Documents", "SenderAgentId");
            CreateIndex("dbo.TemplateDocumentIncomingDetails", "SenderAgentId");
            CreateIndex("dbo.TemplateDocumentIncomingDetails", "Id");
            CreateIndex("dbo.DocumentIncomingDetails", "SenderAgentId");
            CreateIndex("dbo.DocumentIncomingDetails", "Id");
            CreateIndex("dbo.DictionaryStandartSendLists", "PositionId");
            AddForeignKey("dbo.TemplateDocumentIncomingDetails", "Id", "dbo.TemplateDocuments", "Id");
            AddForeignKey("dbo.TemplateDocumentIncomingDetails", "SenderAgentId", "dbo.DictionaryAgents", "Id");
            AddForeignKey("dbo.DocumentIncomingDetails", "Id", "dbo.Documents", "Id");
            AddForeignKey("dbo.DocumentIncomingDetails", "SenderAgentId", "dbo.DictionaryAgents", "Id");
        }
    }
}
