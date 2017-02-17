namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TemplateDocumentAccesses : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "DMS.TemplateDocumentAccesses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        PositionId = c.Int(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.TemplateDocuments", t => t.DocumentId)
                .ForeignKey("DMS.DictionaryPositions", t => t.PositionId)
                .Index(t => new { t.DocumentId, t.PositionId }, unique: true, name: "IX_DocumentPosition")
                .Index(t => t.PositionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.TemplateDocumentAccesses", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.TemplateDocumentAccesses", "DocumentId", "DMS.TemplateDocuments");
            DropIndex("DMS.TemplateDocumentAccesses", new[] { "PositionId" });
            DropIndex("DMS.TemplateDocumentAccesses", "IX_DocumentPosition");
            DropTable("DMS.TemplateDocumentAccesses");
        }
    }
}
