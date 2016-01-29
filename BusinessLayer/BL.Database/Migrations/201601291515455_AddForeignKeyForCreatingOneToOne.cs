namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddForeignKeyForCreatingOneToOne : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.DocumentIncomingDetails");
            DropPrimaryKey("dbo.TemplateDocumentIncomingDetails");
            AlterColumn("dbo.DocumentIncomingDetails", "Id", c => c.Int(nullable: false));
            AlterColumn("dbo.TemplateDocumentIncomingDetails", "Id", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.DocumentIncomingDetails", "Id");
            AddPrimaryKey("dbo.TemplateDocumentIncomingDetails", "Id");
            CreateIndex("dbo.DocumentIncomingDetails", "Id");
            CreateIndex("dbo.TemplateDocumentIncomingDetails", "Id");
            AddForeignKey("dbo.DocumentIncomingDetails", "Id", "dbo.Documents", "Id");
            AddForeignKey("dbo.TemplateDocumentIncomingDetails", "Id", "dbo.TemplateDocuments", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TemplateDocumentIncomingDetails", "Id", "dbo.TemplateDocuments");
            DropForeignKey("dbo.DocumentIncomingDetails", "Id", "dbo.Documents");
            DropIndex("dbo.TemplateDocumentIncomingDetails", new[] { "Id" });
            DropIndex("dbo.DocumentIncomingDetails", new[] { "Id" });
            DropPrimaryKey("dbo.TemplateDocumentIncomingDetails");
            DropPrimaryKey("dbo.DocumentIncomingDetails");
            AlterColumn("dbo.TemplateDocumentIncomingDetails", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.DocumentIncomingDetails", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.TemplateDocumentIncomingDetails", "Id");
            AddPrimaryKey("dbo.DocumentIncomingDetails", "Id");
        }
    }
}
