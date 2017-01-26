namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LogDate1_1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("DMS.SystemLogs", "LogDate1", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("DMS.SystemLogs", "LogDate1", c => c.DateTime(nullable: false));
        }
    }
}
