namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EventAccessId : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.DocumentEvents", "EventAccessId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("DMS.DocumentEvents", "EventAccessId");
        }
    }
}
