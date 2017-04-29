namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EventAccessesAddFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.DocumentEventAccesses", "CountNewEvents", c => c.Int());
            AddColumn("DMS.DocumentEventAccesses", "CountWaits", c => c.Int());
            AddColumn("DMS.DocumentEventAccesses", "OverDueCountWaits", c => c.Int());
            AddColumn("DMS.DocumentEventAccesses", "MinDueDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("DMS.DocumentEventAccesses", "MinDueDate");
            DropColumn("DMS.DocumentEventAccesses", "OverDueCountWaits");
            DropColumn("DMS.DocumentEventAccesses", "CountWaits");
            DropColumn("DMS.DocumentEventAccesses", "CountNewEvents");
        }
    }
}
