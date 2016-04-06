namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RedesignPaperEvents2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("DMS.DocumentPaperEvents", "EventId", "DMS.DocumentEvents");
            DropForeignKey("DMS.DocumentPaperEvents", "EventTypeId", "DMS.DictionaryEventTypes");
            DropForeignKey("DMS.DocumentPaperEvents", "PaperId", "DMS.DocumentPapers");
            DropForeignKey("DMS.DocumentPaperEvents", "PaperListId", "DMS.DocumentPaperLists");
            DropForeignKey("DMS.DocumentPaperEvents", "PlanAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentPaperEvents", "RecieveAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentPaperEvents", "SendAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentPaperEvents", "SendListId", "DMS.DocumentSendLists");
            DropForeignKey("DMS.DocumentPaperEvents", "SourceAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentPaperEvents", "SourcePositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentPaperEvents", "SourcePositionExecutorAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentPaperEvents", "TargetAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentPaperEvents", "TargetPositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentPaperEvents", "TargetPositionExecutorAgentId", "DMS.DictionaryAgents");
            DropIndex("DMS.DocumentPaperEvents", new[] { "PaperId" });
            DropIndex("DMS.DocumentPaperEvents", new[] { "EventTypeId" });
            DropIndex("DMS.DocumentPaperEvents", new[] { "SendListId" });
            DropIndex("DMS.DocumentPaperEvents", new[] { "EventId" });
            DropIndex("DMS.DocumentPaperEvents", new[] { "SourcePositionId" });
            DropIndex("DMS.DocumentPaperEvents", new[] { "SourcePositionExecutorAgentId" });
            DropIndex("DMS.DocumentPaperEvents", new[] { "SourceAgentId" });
            DropIndex("DMS.DocumentPaperEvents", new[] { "TargetPositionId" });
            DropIndex("DMS.DocumentPaperEvents", new[] { "TargetPositionExecutorAgentId" });
            DropIndex("DMS.DocumentPaperEvents", new[] { "TargetAgentId" });
            DropIndex("DMS.DocumentPaperEvents", new[] { "PaperListId" });
            DropIndex("DMS.DocumentPaperEvents", new[] { "PlanAgentId" });
            DropIndex("DMS.DocumentPaperEvents", new[] { "SendAgentId" });
            DropIndex("DMS.DocumentPaperEvents", new[] { "RecieveAgentId" });
        }
        
        public override void Down()
        {
            CreateIndex("DMS.DocumentPaperEvents", "RecieveAgentId");
            CreateIndex("DMS.DocumentPaperEvents", "SendAgentId");
            CreateIndex("DMS.DocumentPaperEvents", "PlanAgentId");
            CreateIndex("DMS.DocumentPaperEvents", "PaperListId");
            CreateIndex("DMS.DocumentPaperEvents", "TargetAgentId");
            CreateIndex("DMS.DocumentPaperEvents", "TargetPositionExecutorAgentId");
            CreateIndex("DMS.DocumentPaperEvents", "TargetPositionId");
            CreateIndex("DMS.DocumentPaperEvents", "SourceAgentId");
            CreateIndex("DMS.DocumentPaperEvents", "SourcePositionExecutorAgentId");
            CreateIndex("DMS.DocumentPaperEvents", "SourcePositionId");
            CreateIndex("DMS.DocumentPaperEvents", "EventId");
            CreateIndex("DMS.DocumentPaperEvents", "SendListId");
            CreateIndex("DMS.DocumentPaperEvents", "EventTypeId");
            CreateIndex("DMS.DocumentPaperEvents", "PaperId");
            AddForeignKey("DMS.DocumentPaperEvents", "TargetPositionExecutorAgentId", "DMS.DictionaryAgents", "Id");
            AddForeignKey("DMS.DocumentPaperEvents", "TargetPositionId", "DMS.DictionaryPositions", "Id");
            AddForeignKey("DMS.DocumentPaperEvents", "TargetAgentId", "DMS.DictionaryAgents", "Id");
            AddForeignKey("DMS.DocumentPaperEvents", "SourcePositionExecutorAgentId", "DMS.DictionaryAgents", "Id");
            AddForeignKey("DMS.DocumentPaperEvents", "SourcePositionId", "DMS.DictionaryPositions", "Id");
            AddForeignKey("DMS.DocumentPaperEvents", "SourceAgentId", "DMS.DictionaryAgents", "Id");
            AddForeignKey("DMS.DocumentPaperEvents", "SendListId", "DMS.DocumentSendLists", "Id");
            AddForeignKey("DMS.DocumentPaperEvents", "SendAgentId", "DMS.DictionaryAgents", "Id");
            AddForeignKey("DMS.DocumentPaperEvents", "RecieveAgentId", "DMS.DictionaryAgents", "Id");
            AddForeignKey("DMS.DocumentPaperEvents", "PlanAgentId", "DMS.DictionaryAgents", "Id");
            AddForeignKey("DMS.DocumentPaperEvents", "PaperListId", "DMS.DocumentPaperLists", "Id");
            AddForeignKey("DMS.DocumentPaperEvents", "PaperId", "DMS.DocumentPapers", "Id");
            AddForeignKey("DMS.DocumentPaperEvents", "EventTypeId", "DMS.DictionaryEventTypes", "Id");
            AddForeignKey("DMS.DocumentPaperEvents", "EventId", "DMS.DocumentEvents", "Id");
        }
    }
}
