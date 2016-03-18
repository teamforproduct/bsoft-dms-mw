namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DocumentTasks : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.DocumentTasks", new[] { "PositionId" });
            AddColumn("dbo.DocumentTasks", "PositionExecutorAgentId", c => c.Int(nullable: false));
            AddColumn("dbo.DocumentTasks", "AgentId", c => c.Int(nullable: false));
            AlterColumn("dbo.DocumentTasks", "PositionId", c => c.Int(nullable: false));
            CreateIndex("dbo.DocumentTasks", "PositionId");
            CreateIndex("dbo.DocumentTasks", "PositionExecutorAgentId");
            CreateIndex("dbo.DocumentTasks", "AgentId");
            AddForeignKey("dbo.DocumentTasks", "AgentId", "dbo.DictionaryAgents", "Id");
            AddForeignKey("dbo.DocumentTasks", "PositionExecutorAgentId", "dbo.DictionaryAgents", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DocumentTasks", "PositionExecutorAgentId", "dbo.DictionaryAgents");
            DropForeignKey("dbo.DocumentTasks", "AgentId", "dbo.DictionaryAgents");
            DropIndex("dbo.DocumentTasks", new[] { "AgentId" });
            DropIndex("dbo.DocumentTasks", new[] { "PositionExecutorAgentId" });
            DropIndex("dbo.DocumentTasks", new[] { "PositionId" });
            AlterColumn("dbo.DocumentTasks", "PositionId", c => c.Int());
            DropColumn("dbo.DocumentTasks", "AgentId");
            DropColumn("dbo.DocumentTasks", "PositionExecutorAgentId");
            CreateIndex("dbo.DocumentTasks", "PositionId");
        }
    }
}
