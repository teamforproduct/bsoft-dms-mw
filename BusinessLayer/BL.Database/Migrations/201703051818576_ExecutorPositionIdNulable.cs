namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExecutorPositionIdNulable : DbMigration
    {
        public override void Up()
        {
            DropIndex("DMS.Documents", new[] { "ExecutorPositionId" });
            DropIndex("DMS.DocumentTasks", new[] { "PositionId" });
            DropIndex("DMS.DocumentFiles", new[] { "ExecutorPositionId" });
            DropIndex("DMS.DocumentLinks", new[] { "ExecutorPositionId" });
            AlterColumn("DMS.Documents", "ExecutorPositionId", c => c.Int());
            AlterColumn("DMS.DocumentTasks", "PositionId", c => c.Int());
            AlterColumn("DMS.DocumentFiles", "ExecutorPositionId", c => c.Int());
            AlterColumn("DMS.DocumentLinks", "ExecutorPositionId", c => c.Int());
            CreateIndex("DMS.Documents", "ExecutorPositionId");
            CreateIndex("DMS.DocumentTasks", "PositionId");
            CreateIndex("DMS.DocumentFiles", "ExecutorPositionId");
            CreateIndex("DMS.DocumentLinks", "ExecutorPositionId");
        }
        
        public override void Down()
        {
            DropIndex("DMS.DocumentLinks", new[] { "ExecutorPositionId" });
            DropIndex("DMS.DocumentFiles", new[] { "ExecutorPositionId" });
            DropIndex("DMS.DocumentTasks", new[] { "PositionId" });
            DropIndex("DMS.Documents", new[] { "ExecutorPositionId" });
            AlterColumn("DMS.DocumentLinks", "ExecutorPositionId", c => c.Int(nullable: false));
            AlterColumn("DMS.DocumentFiles", "ExecutorPositionId", c => c.Int(nullable: false));
            AlterColumn("DMS.DocumentTasks", "PositionId", c => c.Int(nullable: false));
            AlterColumn("DMS.Documents", "ExecutorPositionId", c => c.Int(nullable: false));
            CreateIndex("DMS.DocumentLinks", "ExecutorPositionId");
            CreateIndex("DMS.DocumentFiles", "ExecutorPositionId");
            CreateIndex("DMS.DocumentTasks", "PositionId");
            CreateIndex("DMS.Documents", "ExecutorPositionId");
        }
    }
}
