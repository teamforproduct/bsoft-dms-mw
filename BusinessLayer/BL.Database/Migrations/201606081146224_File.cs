namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class File : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.DocumentFiles", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("DMS.DocumentFiles", "IsWorkedOut", c => c.Boolean(nullable: false));
            AddColumn("DMS.DocumentFiles", "Description", c => c.String(maxLength: 2000));
            AddColumn("DMS.TemplateDocumentFiles", "Description", c => c.String(maxLength: 2000));
        }
        
        public override void Down()
        {
            DropColumn("DMS.TemplateDocumentFiles", "Description");
            DropColumn("DMS.DocumentFiles", "Description");
            DropColumn("DMS.DocumentFiles", "IsWorkedOut");
            DropColumn("DMS.DocumentFiles", "IsDeleted");
        }
    }
}
