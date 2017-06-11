namespace DMS_WebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixSessions : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.SessionLogs", new[] { "LogDate" });
            AddColumn("dbo.SessionLogs", "Date", c => c.DateTime(nullable: false));
            CreateIndex("dbo.SessionLogs", "Date");
            DropColumn("dbo.SessionLogs", "LogDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SessionLogs", "LogDate", c => c.DateTime(nullable: false));
            DropIndex("dbo.SessionLogs", new[] { "Date" });
            DropColumn("dbo.SessionLogs", "Date");
            CreateIndex("dbo.SessionLogs", "LogDate");
        }
    }
}
