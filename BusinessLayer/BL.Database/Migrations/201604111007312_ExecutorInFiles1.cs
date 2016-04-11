namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExecutorInFiles1 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "DMS.DocumentFiles", name: "ExecutorPositionExecutorAgentId", newName: "ExecutorPositionExeAgentId");
            RenameIndex(table: "DMS.DocumentFiles", name: "IX_ExecutorPositionExecutorAgentId", newName: "IX_ExecutorPositionExeAgentId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "DMS.DocumentFiles", name: "IX_ExecutorPositionExeAgentId", newName: "IX_ExecutorPositionExecutorAgentId");
            RenameColumn(table: "DMS.DocumentFiles", name: "ExecutorPositionExeAgentId", newName: "ExecutorPositionExecutorAgentId");
        }
    }
}
