namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sendList : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TemplateDocumentSendListsSet", "DocumentId", "dbo.AdminAccessLevels");
            DropForeignKey("dbo.TemplateDocumentSendListsSet", "DocumentId", "dbo.DictionarySendTypes");
            DropForeignKey("dbo.TemplateDocumentSendListsSet", "DocumentId", "dbo.DictionaryPositions");
            DropForeignKey("dbo.TemplateDocumentSendListsSet", "DocumentId", "dbo.TemplateDocuments");
            DropIndex("dbo.TemplateDocumentSendListsSet", new[] { "DocumentId" });
            AddColumn("dbo.TemplateDocumentSendListsSet", "TemplateDocuments_Id", c => c.Int());
            CreateIndex("dbo.TemplateDocumentSendListsSet", "TemplateDocuments_Id");
            AddForeignKey("dbo.TemplateDocumentSendListsSet", "TemplateDocuments_Id", "dbo.TemplateDocuments", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TemplateDocumentSendListsSet", "TemplateDocuments_Id", "dbo.TemplateDocuments");
            DropIndex("dbo.TemplateDocumentSendListsSet", new[] { "TemplateDocuments_Id" });
            DropColumn("dbo.TemplateDocumentSendListsSet", "TemplateDocuments_Id");
            CreateIndex("dbo.TemplateDocumentSendListsSet", "DocumentId");
            AddForeignKey("dbo.TemplateDocumentSendListsSet", "DocumentId", "dbo.TemplateDocuments", "Id");
            AddForeignKey("dbo.TemplateDocumentSendListsSet", "DocumentId", "dbo.DictionaryPositions", "Id");
            AddForeignKey("dbo.TemplateDocumentSendListsSet", "DocumentId", "dbo.DictionarySendTypes", "Id");
            AddForeignKey("dbo.TemplateDocumentSendListsSet", "DocumentId", "dbo.AdminAccessLevels", "Id");
        }
    }
}
