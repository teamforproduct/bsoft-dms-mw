namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Change2FKByIncorrectFields : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DocumentLinks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        ParentDocumentId = c.Int(nullable: false),
                        LinkTypeId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Documents", t => t.DocumentId)
                .ForeignKey("dbo.DictionaryLinkTypes", t => t.LinkTypeId)
                .ForeignKey("dbo.Documents", t => t.ParentDocumentId)
                .Index(t => t.DocumentId)
                .Index(t => t.ParentDocumentId)
                .Index(t => t.LinkTypeId);
            
            CreateIndex("dbo.DictionaryAgentPersons", "AgentId");
            CreateIndex("dbo.DictionaryAgentPersons", "PersonAgentId");
            CreateIndex("dbo.DictionaryDepartments", "ChiefPositionId");
            CreateIndex("dbo.DocumentSubscriptions", "SendEventId");
            CreateIndex("dbo.DocumentSubscriptions", "DoneEventId");
            AddForeignKey("dbo.DictionaryAgentPersons", "AgentId", "dbo.DictionaryAgents", "Id");
            AddForeignKey("dbo.DictionaryAgentPersons", "PersonAgentId", "dbo.DictionaryAgents", "Id");
            AddForeignKey("dbo.DictionaryDepartments", "ChiefPositionId", "dbo.DictionaryPositions", "Id");
            AddForeignKey("dbo.DocumentSubscriptions", "DoneEventId", "dbo.DocumentEvents", "Id");
            AddForeignKey("dbo.DocumentSubscriptions", "SendEventId", "dbo.DocumentEvents", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DocumentLinks", "ParentDocumentId", "dbo.Documents");
            DropForeignKey("dbo.DocumentLinks", "LinkTypeId", "dbo.DictionaryLinkTypes");
            DropForeignKey("dbo.DocumentLinks", "DocumentId", "dbo.Documents");
            DropForeignKey("dbo.DocumentSubscriptions", "SendEventId", "dbo.DocumentEvents");
            DropForeignKey("dbo.DocumentSubscriptions", "DoneEventId", "dbo.DocumentEvents");
            DropForeignKey("dbo.DictionaryDepartments", "ChiefPositionId", "dbo.DictionaryPositions");
            DropForeignKey("dbo.DictionaryAgentPersons", "PersonAgentId", "dbo.DictionaryAgents");
            DropForeignKey("dbo.DictionaryAgentPersons", "AgentId", "dbo.DictionaryAgents");
            DropIndex("dbo.DocumentLinks", new[] { "LinkTypeId" });
            DropIndex("dbo.DocumentLinks", new[] { "ParentDocumentId" });
            DropIndex("dbo.DocumentLinks", new[] { "DocumentId" });
            DropIndex("dbo.DocumentSubscriptions", new[] { "DoneEventId" });
            DropIndex("dbo.DocumentSubscriptions", new[] { "SendEventId" });
            DropIndex("dbo.DictionaryDepartments", new[] { "ChiefPositionId" });
            DropIndex("dbo.DictionaryAgentPersons", new[] { "PersonAgentId" });
            DropIndex("dbo.DictionaryAgentPersons", new[] { "AgentId" });
            DropTable("dbo.DocumentLinks");
        }
    }
}
