namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExecutorInFiles2 : DbMigration
    {
        public override void Up()
        {
            DropIndex("DMS.DocumentFiles", new[] { "ExecutorPositionId" });
            DropIndex("DMS.DocumentFiles", new[] { "ExecutorPositionExeAgentId" });
            AlterColumn("DMS.DocumentFiles", "ExecutorPositionId", c => c.Int(nullable: false));
            AlterColumn("DMS.DocumentFiles", "ExecutorPositionExeAgentId", c => c.Int(nullable: false));
            CreateIndex("DMS.DocumentFiles", "ExecutorPositionId");
            CreateIndex("DMS.DocumentFiles", "ExecutorPositionExeAgentId");
        }
        
        public override void Down()
        {
            DropIndex("DMS.DocumentFiles", new[] { "ExecutorPositionExeAgentId" });
            DropIndex("DMS.DocumentFiles", new[] { "ExecutorPositionId" });
            AlterColumn("DMS.DocumentFiles", "ExecutorPositionExeAgentId", c => c.Int());
            AlterColumn("DMS.DocumentFiles", "ExecutorPositionId", c => c.Int());
            CreateIndex("DMS.DocumentFiles", "ExecutorPositionExeAgentId");
            CreateIndex("DMS.DocumentFiles", "ExecutorPositionId");
        }
    }
}
