namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Task : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DocumentEvents", "Task", c => c.String());
            DropColumn("dbo.DocumentWaits", "Task");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DocumentWaits", "Task", c => c.String());
            DropColumn("dbo.DocumentEvents", "Task");
        }
    }
}
