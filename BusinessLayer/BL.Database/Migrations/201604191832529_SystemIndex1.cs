namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SystemIndex1 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("DMS.SystemSettings", "ExecutorAgentId");
        }
        
        public override void Down()
        {
            DropIndex("DMS.SystemSettings", new[] { "ExecutorAgentId" });
        }
    }
}
