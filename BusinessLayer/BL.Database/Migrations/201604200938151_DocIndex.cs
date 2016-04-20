namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DocIndex : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("DMS.DocumentEventReaders", "AgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentEventReaders", "EventId", "DMS.DocumentEvents");
            DropForeignKey("DMS.DocumentEventReaders", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentEventReaders", "ReadAgentId", "DMS.DictionaryAgents");
            DropIndex("DMS.DocumentSavedFilters", new[] { "PositionId" });
            DropIndex("DMS.DocumentTags", new[] { "DocumentId" });
            DropIndex("DMS.DocumentTags", new[] { "TagId" });
            DropIndex("DMS.DocumentAccesses", new[] { "DocumentId" });
            DropIndex("DMS.DocumentAccesses", new[] { "PositionId" });
            DropIndex("DMS.DocumentTasks", new[] { "DocumentId" });
            DropIndex("DMS.DocumentEventReaders", new[] { "EventId" });
            DropIndex("DMS.DocumentEventReaders", new[] { "PositionId" });
            DropIndex("DMS.DocumentEventReaders", new[] { "AgentId" });
            DropIndex("DMS.DocumentEventReaders", new[] { "ReadAgentId" });
            DropIndex("DMS.DocumentPapers", new[] { "DocumentId" });
            DropIndex("DMS.DocumentFiles", new[] { "DocumentId" });
            DropIndex("DMS.DocumentLinks", new[] { "DocumentId" });
            DropIndex("DMS.DocumentLinks", new[] { "ParentDocumentId" });
            DropIndex("DMS.DocumentRestrictedSendLists", new[] { "DocumentId" });
            DropIndex("DMS.DocumentRestrictedSendLists", new[] { "PositionId" });
            CreateIndex("DMS.DocumentSavedFilters", new[] { "Icon", "PositionId" }, unique: true, name: "IX_IconPosition");
            CreateIndex("DMS.DocumentSavedFilters", "PositionId");
            CreateIndex("DMS.DocumentTags", new[] { "DocumentId", "TagId" }, unique: true, name: "IX_DocumentTag");
            CreateIndex("DMS.DocumentTags", "TagId");
            CreateIndex("DMS.DocumentAccesses", new[] { "DocumentId", "PositionId" }, unique: true, name: "IX_DocumentPosition");
            CreateIndex("DMS.DocumentAccesses", "PositionId");
            CreateIndex("DMS.DocumentEvents", "Date");
            CreateIndex("DMS.DocumentTasks", new[] { "DocumentId", "Task" }, unique: true, name: "IX_DocumentTask");
            CreateIndex("DMS.DocumentPapers", new[] { "DocumentId", "Name", "IsMain", "IsOriginal", "IsCopy", "OrderNumber" }, unique: true, name: "IX_DocumentNameOrderNumber");
            CreateIndex("DMS.DocumentFiles", new[] { "DocumentId", "Name", "Extension", "Version" }, unique: true, name: "IX_DocumentNameExtensionVersion");
            CreateIndex("DMS.DocumentFiles", new[] { "DocumentId", "OrderNumber", "Version" }, unique: true, name: "IX_DocumentOrderNumberVersion");
            CreateIndex("DMS.DocumentLinks", new[] { "DocumentId", "ParentDocumentId" }, unique: true, name: "IX_DocumentParentDocument");
            CreateIndex("DMS.DocumentLinks", "ParentDocumentId");
            CreateIndex("DMS.DocumentRestrictedSendLists", new[] { "DocumentId", "PositionId" }, unique: true, name: "IX_DocumentPosition");
            CreateIndex("DMS.DocumentRestrictedSendLists", "PositionId");
            DropTable("DMS.DocumentEventReaders");
        }
        
        public override void Down()
        {
            CreateTable(
                "DMS.DocumentEventReaders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EventId = c.Int(nullable: false),
                        PositionId = c.Int(),
                        AgentId = c.Int(),
                        SendDate = c.DateTime(),
                        ReadDate = c.DateTime(),
                        ReadAgentId = c.Int(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropIndex("DMS.DocumentRestrictedSendLists", new[] { "PositionId" });
            DropIndex("DMS.DocumentRestrictedSendLists", "IX_DocumentPosition");
            DropIndex("DMS.DocumentLinks", new[] { "ParentDocumentId" });
            DropIndex("DMS.DocumentLinks", "IX_DocumentParentDocument");
            DropIndex("DMS.DocumentFiles", "IX_DocumentOrderNumberVersion");
            DropIndex("DMS.DocumentFiles", "IX_DocumentNameExtensionVersion");
            DropIndex("DMS.DocumentPapers", "IX_DocumentNameOrderNumber");
            DropIndex("DMS.DocumentTasks", "IX_DocumentTask");
            DropIndex("DMS.DocumentEvents", new[] { "Date" });
            DropIndex("DMS.DocumentAccesses", new[] { "PositionId" });
            DropIndex("DMS.DocumentAccesses", "IX_DocumentPosition");
            DropIndex("DMS.DocumentTags", new[] { "TagId" });
            DropIndex("DMS.DocumentTags", "IX_DocumentTag");
            DropIndex("DMS.DocumentSavedFilters", new[] { "PositionId" });
            DropIndex("DMS.DocumentSavedFilters", "IX_IconPosition");
            CreateIndex("DMS.DocumentRestrictedSendLists", "PositionId");
            CreateIndex("DMS.DocumentRestrictedSendLists", "DocumentId");
            CreateIndex("DMS.DocumentLinks", "ParentDocumentId");
            CreateIndex("DMS.DocumentLinks", "DocumentId");
            CreateIndex("DMS.DocumentFiles", "DocumentId");
            CreateIndex("DMS.DocumentPapers", "DocumentId");
            CreateIndex("DMS.DocumentEventReaders", "ReadAgentId");
            CreateIndex("DMS.DocumentEventReaders", "AgentId");
            CreateIndex("DMS.DocumentEventReaders", "PositionId");
            CreateIndex("DMS.DocumentEventReaders", "EventId");
            CreateIndex("DMS.DocumentTasks", "DocumentId");
            CreateIndex("DMS.DocumentAccesses", "PositionId");
            CreateIndex("DMS.DocumentAccesses", "DocumentId");
            CreateIndex("DMS.DocumentTags", "TagId");
            CreateIndex("DMS.DocumentTags", "DocumentId");
            CreateIndex("DMS.DocumentSavedFilters", "PositionId");
            AddForeignKey("DMS.DocumentEventReaders", "ReadAgentId", "DMS.DictionaryAgents", "Id");
            AddForeignKey("DMS.DocumentEventReaders", "PositionId", "DMS.DictionaryPositions", "Id");
            AddForeignKey("DMS.DocumentEventReaders", "EventId", "DMS.DocumentEvents", "Id");
            AddForeignKey("DMS.DocumentEventReaders", "AgentId", "DMS.DictionaryAgents", "Id");
        }
    }
}
