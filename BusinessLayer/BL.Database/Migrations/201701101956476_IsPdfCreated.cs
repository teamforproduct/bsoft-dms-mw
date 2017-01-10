namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsPdfCreated : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.DocumentFiles", "IsPdfCreated", c => c.Boolean());
            AddColumn("DMS.DocumentFiles", "LastPdfAccessDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("DMS.DocumentFiles", "LastPdfAccessDate");
            DropColumn("DMS.DocumentFiles", "IsPdfCreated");
        }
    }
}
