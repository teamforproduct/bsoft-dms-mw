namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFieldFileSize : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DocumentFiles", "FileSize", c => c.Int(nullable: false));
            AddColumn("dbo.TemplateDocumentFiles", "FileSize", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TemplateDocumentFiles", "FileSize");
            DropColumn("dbo.DocumentFiles", "FileSize");
        }
    }
}
