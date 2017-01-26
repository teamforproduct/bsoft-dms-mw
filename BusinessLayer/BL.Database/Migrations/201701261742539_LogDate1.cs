namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LogDate1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.SystemLogs", "LogDate1", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("DMS.SystemLogs", "LogDate1");
        }
    }
}
