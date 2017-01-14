namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsPdfCreatedInTemplates : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.TemplateDocumentFiles", "IsPdfCreated", c => c.Boolean());
            AddColumn("DMS.TemplateDocumentFiles", "LastPdfAccessDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("DMS.TemplateDocumentFiles", "LastPdfAccessDate");
            DropColumn("DMS.TemplateDocumentFiles", "IsPdfCreated");
        }
    }
}
