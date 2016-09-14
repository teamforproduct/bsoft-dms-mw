namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class digital_signature_2 : DbMigration
    {
        public override void Up()
        {
            DropIndex("DMS.DocumentFiles", new[] { "TypeId" });
            DropIndex("DMS.TemplateDocumentFiles", new[] { "TypeId" });
            AlterColumn("DMS.DocumentFiles", "TypeId", c => c.Int(nullable: false));
            AlterColumn("DMS.TemplateDocumentFiles", "TypeId", c => c.Int(nullable: false));
            CreateIndex("DMS.DocumentFiles", "TypeId");
            CreateIndex("DMS.TemplateDocumentFiles", "TypeId");
        }
        
        public override void Down()
        {
            DropIndex("DMS.TemplateDocumentFiles", new[] { "TypeId" });
            DropIndex("DMS.DocumentFiles", new[] { "TypeId" });
            AlterColumn("DMS.TemplateDocumentFiles", "TypeId", c => c.Int());
            AlterColumn("DMS.DocumentFiles", "TypeId", c => c.Int());
            CreateIndex("DMS.TemplateDocumentFiles", "TypeId");
            CreateIndex("DMS.DocumentFiles", "TypeId");
        }
    }
}
