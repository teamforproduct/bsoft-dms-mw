namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PositionExecutorAgent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DocumentWaits", "TargetDescription", c => c.String());
            AddColumn("dbo.DocumentWaits", "TargetAttentionDate", c => c.DateTime());
            CreateIndex("dbo.Documents", "ExecutorPositionExecutorAgentId");
            CreateIndex("dbo.DocumentEvents", "SourcePositionExecutorAgentId");
            CreateIndex("dbo.DocumentEvents", "TargetPositionExecutorAgentId");
            CreateIndex("dbo.DocumentSendLists", "SourcePositionExecutorAgentId");
            CreateIndex("dbo.DocumentSendLists", "TargetPositionExecutorAgentId");
            AddForeignKey("dbo.DocumentSendLists", "SourcePositionExecutorAgentId", "dbo.DictionaryAgents", "Id");
            AddForeignKey("dbo.DocumentSendLists", "TargetPositionExecutorAgentId", "dbo.DictionaryAgents", "Id");
            AddForeignKey("dbo.DocumentEvents", "SourcePositionExecutorAgentId", "dbo.DictionaryAgents", "Id");
            AddForeignKey("dbo.DocumentEvents", "TargetPositionExecutorAgentId", "dbo.DictionaryAgents", "Id");
            AddForeignKey("dbo.Documents", "ExecutorPositionExecutorAgentId", "dbo.DictionaryAgents", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Documents", "ExecutorPositionExecutorAgentId", "dbo.DictionaryAgents");
            DropForeignKey("dbo.DocumentEvents", "TargetPositionExecutorAgentId", "dbo.DictionaryAgents");
            DropForeignKey("dbo.DocumentEvents", "SourcePositionExecutorAgentId", "dbo.DictionaryAgents");
            DropForeignKey("dbo.DocumentSendLists", "TargetPositionExecutorAgentId", "dbo.DictionaryAgents");
            DropForeignKey("dbo.DocumentSendLists", "SourcePositionExecutorAgentId", "dbo.DictionaryAgents");
            DropIndex("dbo.DocumentSendLists", new[] { "TargetPositionExecutorAgentId" });
            DropIndex("dbo.DocumentSendLists", new[] { "SourcePositionExecutorAgentId" });
            DropIndex("dbo.DocumentEvents", new[] { "TargetPositionExecutorAgentId" });
            DropIndex("dbo.DocumentEvents", new[] { "SourcePositionExecutorAgentId" });
            DropIndex("dbo.Documents", new[] { "ExecutorPositionExecutorAgentId" });
            DropColumn("dbo.DocumentWaits", "TargetAttentionDate");
            DropColumn("dbo.DocumentWaits", "TargetDescription");
        }
    }
}
