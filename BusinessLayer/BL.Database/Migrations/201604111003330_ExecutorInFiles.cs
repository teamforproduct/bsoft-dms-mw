namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExecutorInFiles : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.DocumentFiles", "ExecutorPositionId", c => c.Int());
            AddColumn("DMS.DocumentFiles", "ExecutorPositionExecutorAgentId", c => c.Int());
            CreateIndex("DMS.DocumentFiles", "ExecutorPositionId");
            CreateIndex("DMS.DocumentFiles", "ExecutorPositionExecutorAgentId");
            AddForeignKey("DMS.DocumentFiles", "ExecutorPositionId", "DMS.DictionaryPositions", "Id");
            AddForeignKey("DMS.DocumentFiles", "ExecutorPositionExecutorAgentId", "DMS.DictionaryAgents", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.DocumentFiles", "ExecutorPositionExecutorAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentFiles", "ExecutorPositionId", "DMS.DictionaryPositions");
            DropIndex("DMS.DocumentFiles", new[] { "ExecutorPositionExecutorAgentId" });
            DropIndex("DMS.DocumentFiles", new[] { "ExecutorPositionId" });
            DropColumn("DMS.DocumentFiles", "ExecutorPositionExecutorAgentId");
            DropColumn("DMS.DocumentFiles", "ExecutorPositionId");
        }
    }
}
