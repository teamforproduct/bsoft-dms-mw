namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TemplateDocumentPapers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "DMS.TemplateDocumentPapers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        Name = c.String(maxLength: 400),
                        Description = c.String(maxLength: 2000),
                        IsMain = c.Boolean(nullable: false),
                        IsOriginal = c.Boolean(nullable: false),
                        IsCopy = c.Boolean(nullable: false),
                        PageQuantity = c.Int(nullable: false),
                        OrderNumber = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.TemplateDocuments", t => t.DocumentId)
                .Index(t => new { t.DocumentId, t.Name, t.IsMain, t.IsOriginal, t.IsCopy, t.OrderNumber }, unique: true, name: "IX_DocumentNameOrderNumber");
            
            AlterColumn("DMS.EncryptionCertificates", "Thumbprint", c => c.String(maxLength: 2000));
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.TemplateDocumentPapers", "DocumentId", "DMS.TemplateDocuments");
            DropIndex("DMS.TemplateDocumentPapers", "IX_DocumentNameOrderNumber");
            AlterColumn("DMS.EncryptionCertificates", "Thumbprint", c => c.String());
            DropTable("DMS.TemplateDocumentPapers");
        }
    }
}
