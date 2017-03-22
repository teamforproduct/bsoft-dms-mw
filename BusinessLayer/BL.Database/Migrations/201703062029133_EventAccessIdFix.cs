namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EventAccessIdFix : DbMigration
    {
        public override void Up()
        {
            DropColumn("DMS.DocumentEvents", "EventAccessId");
        }
        
        public override void Down()
        {
            AddColumn("DMS.DocumentEvents", "EventAccessId", c => c.Int());
        }
    }
}
