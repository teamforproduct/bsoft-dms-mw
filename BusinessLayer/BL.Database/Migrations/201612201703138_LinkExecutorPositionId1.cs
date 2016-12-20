namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LinkExecutorPositionId1 : DbMigration
    {
        public override void Up()
        {
            DropIndex("DMS.DocumentLinks", new[] { "ExecutorPositionId" });
            DropIndex("DMS.DocumentLinks", new[] { "ExecutorPositionExeAgentId" });
            AlterColumn("DMS.DocumentLinks", "ExecutorPositionId", c => c.Int(nullable: false));
            AlterColumn("DMS.DocumentLinks", "ExecutorPositionExeAgentId", c => c.Int(nullable: false));
            CreateIndex("DMS.DocumentLinks", "ExecutorPositionId");
            CreateIndex("DMS.DocumentLinks", "ExecutorPositionExeAgentId");
        }
        
        public override void Down()
        {
            DropIndex("DMS.DocumentLinks", new[] { "ExecutorPositionExeAgentId" });
            DropIndex("DMS.DocumentLinks", new[] { "ExecutorPositionId" });
            AlterColumn("DMS.DocumentLinks", "ExecutorPositionExeAgentId", c => c.Int());
            AlterColumn("DMS.DocumentLinks", "ExecutorPositionId", c => c.Int());
            CreateIndex("DMS.DocumentLinks", "ExecutorPositionExeAgentId");
            CreateIndex("DMS.DocumentLinks", "ExecutorPositionId");
        }
    }
}
