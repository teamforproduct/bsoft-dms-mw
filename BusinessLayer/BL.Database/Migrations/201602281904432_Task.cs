namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Task : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DocumentEvents", "TaskName", c => c.String());
            DropColumn("dbo.DocumentWaits", "TaskName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DocumentWaits", "TaskName", c => c.String());
            DropColumn("dbo.DocumentEvents", "TaskName");
        }
    }
}
