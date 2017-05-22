namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPdfAcceptable : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.DocumentFiles", "PdfAcceptable", c => c.Boolean());
            AddColumn("DMS.TemplateDocumentFiles", "PdfAcceptable", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("DMS.TemplateDocumentFiles", "PdfAcceptable");
            DropColumn("DMS.DocumentFiles", "PdfAcceptable");
        }
    }
}
