namespace DMS_WebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixSessions2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SessionLogs", "Session", c => c.String(maxLength: 40));
            CreateIndex("dbo.SessionLogs", "Session", name: "IX_Identifier");
        }
        
        public override void Down()
        {
            DropIndex("dbo.SessionLogs", "IX_Identifier");
            DropColumn("dbo.SessionLogs", "Session");
        }
    }
}
