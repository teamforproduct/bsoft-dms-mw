namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixSystemLogs : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("DMS.SystemLogs", "ExecutorAgentId", "DMS.DictionaryAgents");
            DropIndex("DMS.SystemLogs", new[] { "ExecutorAgentId" });
        }
        
        public override void Down()
        {
            CreateIndex("DMS.SystemLogs", "ExecutorAgentId");
            AddForeignKey("DMS.SystemLogs", "ExecutorAgentId", "DMS.DictionaryAgents", "Id");
        }
    }
}
