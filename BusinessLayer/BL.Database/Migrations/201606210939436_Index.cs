namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Index : DbMigration
    {
        public override void Up()
        {
            DropIndex("DMS.DictionaryAgents", "IX_Name");
            DropIndex("DMS.DictionaryTags", "IX_Name");
            DropIndex("DMS.DictionaryTags", "IX_PositionName");
            DropIndex("DMS.Documents", new[] { "TemplateDocumentId" });
            DropIndex("DMS.DocumentAccesses", new[] { "PositionId" });
            DropIndex("DMS.DocumentEvents", new[] { "DocumentId" });
            DropIndex("DMS.DocumentEvents", new[] { "TaskId" });
            DropIndex("DMS.DocumentEvents", new[] { "SourcePositionId" });
            DropIndex("DMS.DocumentEvents", new[] { "TargetPositionId" });
            DropIndex("DMS.DocumentTaskAccesses", new[] { "PositionId" });
            AlterColumn("DMS.DocumentFiles", "IsWorkedOut", c => c.Boolean());
            CreateIndex("DMS.DictionaryTags", new[] { "PositionId", "Name", "ClientId" }, unique: true, name: "IX_PositionName");
            CreateIndex("DMS.Documents", new[] { "IsRegistered", "Id", "TemplateDocumentId" }, name: "IX_IsRegistered");
            CreateIndex("DMS.Documents", "CreateDate");
            CreateIndex("DMS.DocumentAccesses", new[] { "PositionId", "DocumentId" }, unique: true, name: "IX_PositionDocument");
            CreateIndex("DMS.DocumentEvents", new[] { "ReadDate", "TargetPositionId", "DocumentId", "SourcePositionId" }, name: "IX_DocumentEvents_ReadDate");
            CreateIndex("DMS.DocumentEvents", new[] { "IsAvailableWithinTask", "TaskId" }, name: "IX_DocumentEvents_IsAvailableWithinTask");
            CreateIndex("DMS.DocumentEvents", "LastChangeDate");
            CreateIndex("DMS.DocumentTaskAccesses", new[] { "PositionId", "TaskId" }, unique: true, name: "IX_PositionTask");
            CreateIndex("DMS.DocumentWaits", "DueDate");
            CreateIndex("DMS.DocumentFiles", "LastChangeDate");
        }
        
        public override void Down()
        {
            DropIndex("DMS.DocumentFiles", new[] { "LastChangeDate" });
            DropIndex("DMS.DocumentWaits", new[] { "DueDate" });
            DropIndex("DMS.DocumentTaskAccesses", "IX_PositionTask");
            DropIndex("DMS.DocumentEvents", new[] { "LastChangeDate" });
            DropIndex("DMS.DocumentEvents", "IX_DocumentEvents_IsAvailableWithinTask");
            DropIndex("DMS.DocumentEvents", "IX_DocumentEvents_ReadDate");
            DropIndex("DMS.DocumentAccesses", "IX_PositionDocument");
            DropIndex("DMS.Documents", new[] { "CreateDate" });
            DropIndex("DMS.Documents", "IX_IsRegistered");
            DropIndex("DMS.DictionaryTags", "IX_PositionName");
            AlterColumn("DMS.DocumentFiles", "IsWorkedOut", c => c.Boolean(nullable: false));
            CreateIndex("DMS.DocumentTaskAccesses", "PositionId");
            CreateIndex("DMS.DocumentEvents", "TargetPositionId");
            CreateIndex("DMS.DocumentEvents", "SourcePositionId");
            CreateIndex("DMS.DocumentEvents", "TaskId");
            CreateIndex("DMS.DocumentEvents", "DocumentId");
            CreateIndex("DMS.DocumentAccesses", "PositionId");
            CreateIndex("DMS.Documents", "TemplateDocumentId");
            CreateIndex("DMS.DictionaryTags", new[] { "PositionId", "Name" }, unique: true, name: "IX_PositionName");
            CreateIndex("DMS.DictionaryTags", "ClientId", unique: true, name: "IX_Name");
            CreateIndex("DMS.DictionaryAgents", new[] { "Name", "ClientId" }, unique: true, name: "IX_Name");
        }
    }
}
