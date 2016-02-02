using System;
using System.Data.Entity.Migrations;

namespace BL.Database.Migrations
{

    public partial class ForeignKeyIncomingDetails : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.DocumentIncomingDetails", "DocumentId");
            CreateIndex("dbo.TemplateDocumentIncomingDetails", "DocumentId");
            AddForeignKey("dbo.DocumentIncomingDetails", "DocumentId", "dbo.Documents", "Id");
            AddForeignKey("dbo.TemplateDocumentIncomingDetails", "DocumentId", "dbo.TemplateDocuments", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TemplateDocumentIncomingDetails", "DocumentId", "dbo.TemplateDocuments");
            DropForeignKey("dbo.DocumentIncomingDetails", "DocumentId", "dbo.Documents");
            DropIndex("dbo.TemplateDocumentIncomingDetails", new[] { "DocumentId" });
            DropIndex("dbo.DocumentIncomingDetails", new[] { "DocumentId" });
        }
    }
}
