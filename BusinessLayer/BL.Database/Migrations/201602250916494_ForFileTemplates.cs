namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ForFileTemplates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TemplateDocumentFiles", "FileType", c => c.String());
            AddColumn("dbo.TemplateDocumentFiles", "IsAdditional", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TemplateDocumentFiles", "IsAdditional");
            DropColumn("dbo.TemplateDocumentFiles", "FileType");
        }
    }
}
