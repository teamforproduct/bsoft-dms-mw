namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LinkExecutorPositionId : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.DocumentLinks", "ExecutorPositionId", c => c.Int());
            AddColumn("DMS.DocumentLinks", "ExecutorPositionExeAgentId", c => c.Int());
            AddColumn("DMS.DocumentLinks", "ExecutorPositionExeTypeId", c => c.Int());
            CreateIndex("DMS.DocumentLinks", "ExecutorPositionId");
            CreateIndex("DMS.DocumentLinks", "ExecutorPositionExeAgentId");
            CreateIndex("DMS.DocumentLinks", "ExecutorPositionExeTypeId");
            AddForeignKey("DMS.DocumentLinks", "ExecutorPositionId", "DMS.DictionaryPositions", "Id");
            AddForeignKey("DMS.DocumentLinks", "ExecutorPositionExeAgentId", "DMS.DictionaryAgents", "Id");
            AddForeignKey("DMS.DocumentLinks", "ExecutorPositionExeTypeId", "DMS.DicPositionExecutorTypes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.DocumentLinks", "ExecutorPositionExeTypeId", "DMS.DicPositionExecutorTypes");
            DropForeignKey("DMS.DocumentLinks", "ExecutorPositionExeAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentLinks", "ExecutorPositionId", "DMS.DictionaryPositions");
            DropIndex("DMS.DocumentLinks", new[] { "ExecutorPositionExeTypeId" });
            DropIndex("DMS.DocumentLinks", new[] { "ExecutorPositionExeAgentId" });
            DropIndex("DMS.DocumentLinks", new[] { "ExecutorPositionId" });
            DropColumn("DMS.DocumentLinks", "ExecutorPositionExeTypeId");
            DropColumn("DMS.DocumentLinks", "ExecutorPositionExeAgentId");
            DropColumn("DMS.DocumentLinks", "ExecutorPositionId");
        }
    }
}
