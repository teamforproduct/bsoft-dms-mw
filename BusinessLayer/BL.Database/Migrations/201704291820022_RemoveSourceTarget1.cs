namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveSourceTarget1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("DMS.DocumentSendLists", "SourceAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentSendLists", "SourcePositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentSendLists", "SourcePositionExecutorAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentSendLists", "SourcePositionExecutorTypeId", "DMS.DicPositionExecutorTypes");
            DropForeignKey("DMS.DocumentSendLists", "TargetAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentSendLists", "TargetPositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentSendLists", "TargetPositionExecutorAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentSendLists", "TargetPositionExecutorTypeId", "DMS.DicPositionExecutorTypes");
            DropForeignKey("DMS.DocumentEvents", "ReadAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentEvents", "SourceAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentEvents", "SourcePositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentEvents", "SourcePositionExecutorAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentEvents", "SourcePositionExecutorTypeId", "DMS.DicPositionExecutorTypes");
            DropForeignKey("DMS.DocumentEvents", "TargetAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentEvents", "TargetPositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentEvents", "TargetPositionExecutorAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentEvents", "TargetPositionExecutorTypeId", "DMS.DicPositionExecutorTypes");
            DropIndex("DMS.DocumentEvents", "IX_ReadDate");
            DropIndex("DMS.DocumentEvents", new[] { "SourcePositionId" });
            DropIndex("DMS.DocumentEvents", new[] { "SourcePositionExecutorAgentId" });
            DropIndex("DMS.DocumentEvents", new[] { "SourcePositionExecutorTypeId" });
            DropIndex("DMS.DocumentEvents", new[] { "SourceAgentId" });
            DropIndex("DMS.DocumentEvents", new[] { "TargetPositionId" });
            DropIndex("DMS.DocumentEvents", new[] { "TargetPositionExecutorAgentId" });
            DropIndex("DMS.DocumentEvents", new[] { "TargetPositionExecutorTypeId" });
            DropIndex("DMS.DocumentEvents", new[] { "TargetAgentId" });
            DropIndex("DMS.DocumentEvents", new[] { "ReadAgentId" });
            DropIndex("DMS.DocumentSendLists", new[] { "SourcePositionId" });
            DropIndex("DMS.DocumentSendLists", new[] { "SourcePositionExecutorAgentId" });
            DropIndex("DMS.DocumentSendLists", new[] { "SourcePositionExecutorTypeId" });
            DropIndex("DMS.DocumentSendLists", new[] { "SourceAgentId" });
            DropIndex("DMS.DocumentSendLists", new[] { "TargetPositionId" });
            DropIndex("DMS.DocumentSendLists", new[] { "TargetPositionExecutorAgentId" });
            DropIndex("DMS.DocumentSendLists", new[] { "TargetPositionExecutorTypeId" });
            DropIndex("DMS.DocumentSendLists", new[] { "TargetAgentId" });
        }
        
        public override void Down()
        {
            CreateIndex("DMS.DocumentSendLists", "TargetAgentId");
            CreateIndex("DMS.DocumentSendLists", "TargetPositionExecutorTypeId");
            CreateIndex("DMS.DocumentSendLists", "TargetPositionExecutorAgentId");
            CreateIndex("DMS.DocumentSendLists", "TargetPositionId");
            CreateIndex("DMS.DocumentSendLists", "SourceAgentId");
            CreateIndex("DMS.DocumentSendLists", "SourcePositionExecutorTypeId");
            CreateIndex("DMS.DocumentSendLists", "SourcePositionExecutorAgentId");
            CreateIndex("DMS.DocumentSendLists", "SourcePositionId");
            CreateIndex("DMS.DocumentEvents", "ReadAgentId");
            CreateIndex("DMS.DocumentEvents", "TargetAgentId");
            CreateIndex("DMS.DocumentEvents", "TargetPositionExecutorTypeId");
            CreateIndex("DMS.DocumentEvents", "TargetPositionExecutorAgentId");
            CreateIndex("DMS.DocumentEvents", "TargetPositionId");
            CreateIndex("DMS.DocumentEvents", "SourceAgentId");
            CreateIndex("DMS.DocumentEvents", "SourcePositionExecutorTypeId");
            CreateIndex("DMS.DocumentEvents", "SourcePositionExecutorAgentId");
            CreateIndex("DMS.DocumentEvents", "SourcePositionId");
            CreateIndex("DMS.DocumentEvents", new[] { "ReadDate", "TargetPositionId", "DocumentId", "SourcePositionId" }, name: "IX_ReadDate");
            AddForeignKey("DMS.DocumentEvents", "TargetPositionExecutorTypeId", "DMS.DicPositionExecutorTypes", "Id");
            AddForeignKey("DMS.DocumentEvents", "TargetPositionExecutorAgentId", "DMS.DictionaryAgents", "Id");
            AddForeignKey("DMS.DocumentEvents", "TargetPositionId", "DMS.DictionaryPositions", "Id");
            AddForeignKey("DMS.DocumentEvents", "TargetAgentId", "DMS.DictionaryAgents", "Id");
            AddForeignKey("DMS.DocumentEvents", "SourcePositionExecutorTypeId", "DMS.DicPositionExecutorTypes", "Id");
            AddForeignKey("DMS.DocumentEvents", "SourcePositionExecutorAgentId", "DMS.DictionaryAgents", "Id");
            AddForeignKey("DMS.DocumentEvents", "SourcePositionId", "DMS.DictionaryPositions", "Id");
            AddForeignKey("DMS.DocumentEvents", "SourceAgentId", "DMS.DictionaryAgents", "Id");
            AddForeignKey("DMS.DocumentEvents", "ReadAgentId", "DMS.DictionaryAgents", "Id");
            AddForeignKey("DMS.DocumentSendLists", "TargetPositionExecutorTypeId", "DMS.DicPositionExecutorTypes", "Id");
            AddForeignKey("DMS.DocumentSendLists", "TargetPositionExecutorAgentId", "DMS.DictionaryAgents", "Id");
            AddForeignKey("DMS.DocumentSendLists", "TargetPositionId", "DMS.DictionaryPositions", "Id");
            AddForeignKey("DMS.DocumentSendLists", "TargetAgentId", "DMS.DictionaryAgents", "Id");
            AddForeignKey("DMS.DocumentSendLists", "SourcePositionExecutorTypeId", "DMS.DicPositionExecutorTypes", "Id");
            AddForeignKey("DMS.DocumentSendLists", "SourcePositionExecutorAgentId", "DMS.DictionaryAgents", "Id");
            AddForeignKey("DMS.DocumentSendLists", "SourcePositionId", "DMS.DictionaryPositions", "Id");
            AddForeignKey("DMS.DocumentSendLists", "SourceAgentId", "DMS.DictionaryAgents", "Id");
        }
    }
}
