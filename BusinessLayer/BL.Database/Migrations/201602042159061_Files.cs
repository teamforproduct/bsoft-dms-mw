namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Files : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TemplateDocumentFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        Name = c.String(),
                        OrderNumber = c.Int(nullable: false),
                        Extention = c.String(),
                        Content = c.Binary(),
                        Hash = c.String(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TemplateDocuments", t => t.DocumentId)
                .Index(t => t.DocumentId);
            
            AddColumn("dbo.DocumentFiles", "OrderNumber", c => c.Int(nullable: false));
            AddColumn("dbo.DocumentFiles", "Version", c => c.Int(nullable: false));
            AddColumn("dbo.DocumentFiles", "Hash", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TemplateDocumentFiles", "DocumentId", "dbo.TemplateDocuments");
            DropIndex("dbo.TemplateDocumentFiles", new[] { "DocumentId" });
            DropColumn("dbo.DocumentFiles", "Hash");
            DropColumn("dbo.DocumentFiles", "Version");
            DropColumn("dbo.DocumentFiles", "OrderNumber");
            DropTable("dbo.TemplateDocumentFiles");
        }
    }
}
