namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeUnIndexDocumentFilesName : DbMigration
    {
        public override void Up()
        {
            DropIndex("DMS.DocumentFiles", "IX_DocumentNameExtensionVersion");
        }
        
        public override void Down()
        {
            CreateIndex("DMS.DocumentFiles", new[] { "DocumentId", "Name", "Extension", "Version" }, unique: true, name: "IX_DocumentNameExtensionVersion");
        }
    }
}
