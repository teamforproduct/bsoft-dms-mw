namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DocumentAccesses : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.DocumentAccesses", "OverDueCountWaits", c => c.Int());
            AddColumn("DMS.DocumentAccesses", "MinDueDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("DMS.DocumentAccesses", "MinDueDate");
            DropColumn("DMS.DocumentAccesses", "OverDueCountWaits");
        }
    }
}
