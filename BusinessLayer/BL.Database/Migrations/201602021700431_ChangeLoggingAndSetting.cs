using System.Data.Entity.Migrations;

namespace BL.Database.Migrations
{
    public partial class ChangeLoggingAndSetting : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.SystemLogs", name: "ExecutorAgent_Id", newName: "ExecutorAgentId");
            RenameIndex(table: "dbo.SystemLogs", name: "IX_ExecutorAgent_Id", newName: "IX_ExecutorAgentId");
            DropPrimaryKey("dbo.SystemSettings");
            AddColumn("dbo.SystemLogs", "LogLevel", c => c.Int(nullable: false));
            AddColumn("dbo.SystemLogs", "LogTrace", c => c.String());
            AddColumn("dbo.SystemLogs", "LogException", c => c.String());
            AddColumn("dbo.SystemSettings", "Id", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.SystemSettings", "ExecutorAgentId", c => c.Int());
            AlterColumn("dbo.SystemSettings", "Key", c => c.String());
            AddPrimaryKey("dbo.SystemSettings", "Id");
            CreateIndex("dbo.SystemSettings", "ExecutorAgentId");
            AddForeignKey("dbo.SystemSettings", "ExecutorAgentId", "dbo.DictionaryAgents", "Id");
            DropColumn("dbo.SystemLogs", "Trace");
            DropColumn("dbo.SystemLogs", "AgentId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SystemLogs", "AgentId", c => c.Int());
            AddColumn("dbo.SystemLogs", "Trace", c => c.String());
            DropForeignKey("dbo.SystemSettings", "ExecutorAgentId", "dbo.DictionaryAgents");
            DropIndex("dbo.SystemSettings", new[] { "ExecutorAgentId" });
            DropPrimaryKey("dbo.SystemSettings");
            AlterColumn("dbo.SystemSettings", "Key", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.SystemSettings", "ExecutorAgentId");
            DropColumn("dbo.SystemSettings", "Id");
            DropColumn("dbo.SystemLogs", "LogException");
            DropColumn("dbo.SystemLogs", "LogTrace");
            DropColumn("dbo.SystemLogs", "LogLevel");
            AddPrimaryKey("dbo.SystemSettings", "Key");
            RenameIndex(table: "dbo.SystemLogs", name: "IX_ExecutorAgentId", newName: "IX_ExecutorAgent_Id");
            RenameColumn(table: "dbo.SystemLogs", name: "ExecutorAgentId", newName: "ExecutorAgent_Id");
        }
    }
}
