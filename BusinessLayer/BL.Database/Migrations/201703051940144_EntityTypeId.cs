namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EntityTypeId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("DMS.DocumentEventAccesses", "AgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentEventAccesses", "DocumentId", "DMS.Documents");
            DropForeignKey("DMS.DocumentEventAccesses", "EventId", "DMS.DocumentEvents");
            DropForeignKey("DMS.DocumentEventAccesses", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentEventAccesses", "ReadAgentId", "DMS.DictionaryAgents");
            DropIndex("DMS.DocumentEventAccesses", new[] { "ClientId" });
            DropIndex("DMS.DocumentEventAccesses", new[] { "DocumentId" });
            DropIndex("DMS.DocumentEventAccesses", new[] { "EventId" });
            DropIndex("DMS.DocumentEventAccesses", new[] { "PositionId" });
            DropIndex("DMS.DocumentEventAccesses", new[] { "AgentId" });
            DropIndex("DMS.DocumentEventAccesses", new[] { "ReadAgentId" });
            AddColumn("DMS.DocumentAccesses", "EntityTypeId", c => c.Int(nullable: false));
            AddColumn("DMS.DocumentSubscriptions", "EntityTypeId", c => c.Int(nullable: false));
            AddColumn("DMS.Documents", "EntityTypeId", c => c.Int(nullable: false));
            AddColumn("DMS.DocumentEvents", "EntityTypeId", c => c.Int(nullable: false));
            AddColumn("DMS.DocumentTasks", "EntityTypeId", c => c.Int(nullable: false));
            AddColumn("DMS.DocumentTaskAccesses", "EntityTypeId", c => c.Int(nullable: false));
            AddColumn("DMS.DocumentTaskAccesses", "DocumentId", c => c.Int(nullable: false));
            AddColumn("DMS.DocumentWaits", "EntityTypeId", c => c.Int(nullable: false));
            AddColumn("DMS.DocumentPapers", "EntityTypeId", c => c.Int(nullable: false));
            AddColumn("DMS.DocumentFiles", "EntityTypeId", c => c.Int(nullable: false));
            AddColumn("DMS.DocumentLinks", "EntityTypeId", c => c.Int(nullable: false));
            AddColumn("DMS.DocumentRestrictedSendLists", "EntityTypeId", c => c.Int(nullable: false));
            AddColumn("DMS.DocumentTags", "EntityTypeId", c => c.Int(nullable: false));
            CreateIndex("DMS.DocumentAccesses", "EntityTypeId");
            CreateIndex("DMS.DocumentSubscriptions", "EntityTypeId");
            CreateIndex("DMS.Documents", "EntityTypeId");
            CreateIndex("DMS.DocumentEvents", "EntityTypeId");
            CreateIndex("DMS.DocumentTasks", "EntityTypeId");
            CreateIndex("DMS.DocumentTaskAccesses", "EntityTypeId");
            CreateIndex("DMS.DocumentWaits", "EntityTypeId");
            CreateIndex("DMS.DocumentPapers", "EntityTypeId");
            CreateIndex("DMS.DocumentFiles", "EntityTypeId");
            CreateIndex("DMS.DocumentLinks", "EntityTypeId");
            CreateIndex("DMS.DocumentRestrictedSendLists", "EntityTypeId");
            CreateIndex("DMS.DocumentTags", "EntityTypeId");
            DropTable("DMS.DocumentEventAccesses");
        }
        
        public override void Down()
        {
            CreateTable(
                "DMS.DocumentEventAccesses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        DocumentId = c.Int(nullable: false),
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
            
            DropIndex("DMS.DocumentTags", new[] { "EntityTypeId" });
            DropIndex("DMS.DocumentRestrictedSendLists", new[] { "EntityTypeId" });
            DropIndex("DMS.DocumentLinks", new[] { "EntityTypeId" });
            DropIndex("DMS.DocumentFiles", new[] { "EntityTypeId" });
            DropIndex("DMS.DocumentPapers", new[] { "EntityTypeId" });
            DropIndex("DMS.DocumentWaits", new[] { "EntityTypeId" });
            DropIndex("DMS.DocumentTaskAccesses", new[] { "EntityTypeId" });
            DropIndex("DMS.DocumentTasks", new[] { "EntityTypeId" });
            DropIndex("DMS.DocumentEvents", new[] { "EntityTypeId" });
            DropIndex("DMS.Documents", new[] { "EntityTypeId" });
            DropIndex("DMS.DocumentSubscriptions", new[] { "EntityTypeId" });
            DropIndex("DMS.DocumentAccesses", new[] { "EntityTypeId" });
            DropColumn("DMS.DocumentTags", "EntityTypeId");
            DropColumn("DMS.DocumentRestrictedSendLists", "EntityTypeId");
            DropColumn("DMS.DocumentLinks", "EntityTypeId");
            DropColumn("DMS.DocumentFiles", "EntityTypeId");
            DropColumn("DMS.DocumentPapers", "EntityTypeId");
            DropColumn("DMS.DocumentWaits", "EntityTypeId");
            DropColumn("DMS.DocumentTaskAccesses", "DocumentId");
            DropColumn("DMS.DocumentTaskAccesses", "EntityTypeId");
            DropColumn("DMS.DocumentTasks", "EntityTypeId");
            DropColumn("DMS.DocumentEvents", "EntityTypeId");
            DropColumn("DMS.Documents", "EntityTypeId");
            DropColumn("DMS.DocumentSubscriptions", "EntityTypeId");
            DropColumn("DMS.DocumentAccesses", "EntityTypeId");
            CreateIndex("DMS.DocumentEventAccesses", "ReadAgentId");
            CreateIndex("DMS.DocumentEventAccesses", "AgentId");
            CreateIndex("DMS.DocumentEventAccesses", "PositionId");
            CreateIndex("DMS.DocumentEventAccesses", "EventId");
            CreateIndex("DMS.DocumentEventAccesses", "DocumentId");
            CreateIndex("DMS.DocumentEventAccesses", "ClientId");
            AddForeignKey("DMS.DocumentEventAccesses", "ReadAgentId", "DMS.DictionaryAgents", "Id");
            AddForeignKey("DMS.DocumentEventAccesses", "PositionId", "DMS.DictionaryPositions", "Id");
            AddForeignKey("DMS.DocumentEventAccesses", "EventId", "DMS.DocumentEvents", "Id");
            AddForeignKey("DMS.DocumentEventAccesses", "DocumentId", "DMS.Documents", "Id");
            AddForeignKey("DMS.DocumentEventAccesses", "AgentId", "DMS.DictionaryAgents", "Id");
        }
    }
}
