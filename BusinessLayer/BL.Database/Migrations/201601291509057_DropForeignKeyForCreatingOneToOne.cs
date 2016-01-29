namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropForeignKeyForCreatingOneToOne : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DocumentIncomingDetails", "DocumentId", "dbo.Documents");
            DropForeignKey("dbo.TemplateDocumentIncomingDetails", "DocumentId", "dbo.TemplateDocuments");
            DropIndex("dbo.DocumentIncomingDetails", new[] { "DocumentId" });
            DropIndex("dbo.TemplateDocumentIncomingDetails", new[] { "DocumentId" });
            DropColumn("dbo.DocumentIncomingDetails", "DocumentId");
            DropColumn("dbo.TemplateDocumentIncomingDetails", "DocumentId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TemplateDocumentIncomingDetails", "DocumentId", c => c.Int(nullable: false));
            AddColumn("dbo.DocumentIncomingDetails", "DocumentId", c => c.Int(nullable: false));
            CreateIndex("dbo.TemplateDocumentIncomingDetails", "DocumentId");
            CreateIndex("dbo.DocumentIncomingDetails", "DocumentId");
            AddForeignKey("dbo.TemplateDocumentIncomingDetails", "DocumentId", "dbo.TemplateDocuments", "Id");
            AddForeignKey("dbo.DocumentIncomingDetails", "DocumentId", "dbo.Documents", "Id");
        }
    }
}
