namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FileSize : DbMigration
    {
        public override void Up()
        {
            AlterColumn("DMS.DocumentFiles", "FileSize", c => c.Long(nullable: false));
            AlterColumn("DMS.TemplateDocumentFiles", "FileSize", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("DMS.TemplateDocumentFiles", "FileSize", c => c.Int(nullable: false));
            AlterColumn("DMS.DocumentFiles", "FileSize", c => c.Int(nullable: false));
        }
    }
}
