namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DocumentEventAccessesSet : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "DMS.DocumentEventAccesses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        EntityTypeId = c.Int(nullable: false),
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.AgentId)
                .ForeignKey("DMS.Documents", t => t.DocumentId)
                .ForeignKey("DMS.DocumentEvents", t => t.EventId)
                .ForeignKey("DMS.DictionaryPositions", t => t.PositionId)
                .ForeignKey("DMS.DictionaryAgents", t => t.ReadAgentId)
                .Index(t => t.ClientId)
                .Index(t => t.EntityTypeId)
                .Index(t => t.DocumentId)
                .Index(t => t.EventId)
                .Index(t => t.PositionId)
                .Index(t => t.AgentId)
                .Index(t => t.ReadAgentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.DocumentEventAccesses", "ReadAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentEventAccesses", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentEventAccesses", "EventId", "DMS.DocumentEvents");
            DropForeignKey("DMS.DocumentEventAccesses", "DocumentId", "DMS.Documents");
            DropForeignKey("DMS.DocumentEventAccesses", "AgentId", "DMS.DictionaryAgents");
            DropIndex("DMS.DocumentEventAccesses", new[] { "ReadAgentId" });
            DropIndex("DMS.DocumentEventAccesses", new[] { "AgentId" });
            DropIndex("DMS.DocumentEventAccesses", new[] { "PositionId" });
            DropIndex("DMS.DocumentEventAccesses", new[] { "EventId" });
            DropIndex("DMS.DocumentEventAccesses", new[] { "DocumentId" });
            DropIndex("DMS.DocumentEventAccesses", new[] { "EntityTypeId" });
            DropIndex("DMS.DocumentEventAccesses", new[] { "ClientId" });
            DropTable("DMS.DocumentEventAccesses");
        }
    }
}
