namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sendList : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TemplateDocumentSendLists", "DocumentId", "dbo.AdminAccessLevels");
            DropForeignKey("dbo.TemplateDocumentSendLists", "DocumentId", "dbo.DictionarySendTypes");
            DropForeignKey("dbo.TemplateDocumentSendLists", "DocumentId", "dbo.DictionaryPositions");
            DropForeignKey("dbo.TemplateDocumentSendLists", "DocumentId", "dbo.TemplateDocuments");
            DropIndex("dbo.TemplateDocumentSendLists", new[] { "DocumentId" });
            AddColumn("dbo.TemplateDocumentSendLists", "TemplateDocuments_Id", c => c.Int());
            CreateIndex("dbo.TemplateDocumentSendLists", "TemplateDocuments_Id");
            AddForeignKey("dbo.TemplateDocumentSendLists", "TemplateDocuments_Id", "dbo.TemplateDocuments", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TemplateDocumentSendLists", "TemplateDocuments_Id", "dbo.TemplateDocuments");
            DropIndex("dbo.TemplateDocumentSendLists", new[] { "TemplateDocuments_Id" });
            DropColumn("dbo.TemplateDocumentSendLists", "TemplateDocuments_Id");
            CreateIndex("dbo.TemplateDocumentSendLists", "DocumentId");
            AddForeignKey("dbo.TemplateDocumentSendLists", "DocumentId", "dbo.TemplateDocuments", "Id");
            AddForeignKey("dbo.TemplateDocumentSendLists", "DocumentId", "dbo.DictionaryPositions", "Id");
            AddForeignKey("dbo.TemplateDocumentSendLists", "DocumentId", "dbo.DictionarySendTypes", "Id");
            AddForeignKey("dbo.TemplateDocumentSendLists", "DocumentId", "dbo.AdminAccessLevels", "Id");
        }
    }
}
