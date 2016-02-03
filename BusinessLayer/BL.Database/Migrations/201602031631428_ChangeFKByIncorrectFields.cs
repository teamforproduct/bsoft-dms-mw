namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeFKByIncorrectFields : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DictionaryAgentPersons", "DictionaryAgents_Id", "dbo.DictionaryAgents");
            DropForeignKey("dbo.DictionaryDepartments", "ChiefPositionId", "dbo.DictionaryPositions");
            DropForeignKey("dbo.DictionaryDepartments", "DictionaryPositions_Id", "dbo.DictionaryPositions");
            DropForeignKey("dbo.DictionaryAgentPersons", "AgentId", "dbo.DictionaryAgents");
            DropForeignKey("dbo.DictionaryAgentPersons", "PersonAgentId", "dbo.DictionaryAgents");
            DropForeignKey("dbo.DocumentSubscriptions", "DoneEventId", "dbo.DocumentEvents");
            DropForeignKey("dbo.DocumentSubscriptions", "SendEventId", "dbo.DocumentEvents");
            DropForeignKey("dbo.DocumentSubscriptions", "DocumentEvents_Id", "dbo.DocumentEvents");
            DropForeignKey("dbo.DocumentSubscriptions", "DocumentEvents_Id1", "dbo.DocumentEvents");
            DropForeignKey("dbo.DocumentLinks", "DocumentId", "dbo.Documents");
            DropForeignKey("dbo.DocumentLinks", "LinkTypeId", "dbo.DictionaryLinkTypes");
            DropForeignKey("dbo.DocumentLinks", "ParentDocumentId", "dbo.Documents");
            DropForeignKey("dbo.DocumentLinks", "Documents_Id", "dbo.Documents");
            DropIndex("dbo.DictionaryAgentPersons", new[] { "AgentId" });
            DropIndex("dbo.DictionaryAgentPersons", new[] { "PersonAgentId" });
            DropIndex("dbo.DictionaryAgentPersons", new[] { "DictionaryAgents_Id" });
            DropIndex("dbo.DictionaryDepartments", new[] { "ChiefPositionId" });
            DropIndex("dbo.DictionaryDepartments", new[] { "DictionaryPositions_Id" });
            DropIndex("dbo.DocumentSubscriptions", new[] { "SendEventId" });
            DropIndex("dbo.DocumentSubscriptions", new[] { "DoneEventId" });
            DropIndex("dbo.DocumentSubscriptions", new[] { "DocumentEvents_Id" });
            DropIndex("dbo.DocumentSubscriptions", new[] { "DocumentEvents_Id1" });
            DropIndex("dbo.DocumentLinks", new[] { "DocumentId" });
            DropIndex("dbo.DocumentLinks", new[] { "ParentDocumentId" });
            DropIndex("dbo.DocumentLinks", new[] { "LinkTypeId" });
            DropIndex("dbo.DocumentLinks", new[] { "Documents_Id" });
            DropColumn("dbo.DictionaryAgentPersons", "DictionaryAgents_Id");
            DropColumn("dbo.DictionaryDepartments", "DictionaryPositions_Id");
            DropColumn("dbo.DocumentSubscriptions", "DocumentEvents_Id");
            DropColumn("dbo.DocumentSubscriptions", "DocumentEvents_Id1");
            DropTable("dbo.DocumentLinks");
        }
        
        public override void Down()
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
                        Documents_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.DocumentSubscriptions", "DocumentEvents_Id1", c => c.Int());
            AddColumn("dbo.DocumentSubscriptions", "DocumentEvents_Id", c => c.Int());
            AddColumn("dbo.DictionaryDepartments", "DictionaryPositions_Id", c => c.Int());
            AddColumn("dbo.DictionaryAgentPersons", "DictionaryAgents_Id", c => c.Int());
            CreateIndex("dbo.DocumentLinks", "Documents_Id");
            CreateIndex("dbo.DocumentLinks", "LinkTypeId");
            CreateIndex("dbo.DocumentLinks", "ParentDocumentId");
            CreateIndex("dbo.DocumentLinks", "DocumentId");
            CreateIndex("dbo.DocumentSubscriptions", "DocumentEvents_Id1");
            CreateIndex("dbo.DocumentSubscriptions", "DocumentEvents_Id");
            CreateIndex("dbo.DocumentSubscriptions", "DoneEventId");
            CreateIndex("dbo.DocumentSubscriptions", "SendEventId");
            CreateIndex("dbo.DictionaryDepartments", "DictionaryPositions_Id");
            CreateIndex("dbo.DictionaryDepartments", "ChiefPositionId");
            CreateIndex("dbo.DictionaryAgentPersons", "DictionaryAgents_Id");
            CreateIndex("dbo.DictionaryAgentPersons", "PersonAgentId");
            CreateIndex("dbo.DictionaryAgentPersons", "AgentId");
            AddForeignKey("dbo.DocumentLinks", "Documents_Id", "dbo.Documents", "Id");
            AddForeignKey("dbo.DocumentLinks", "ParentDocumentId", "dbo.Documents", "Id");
            AddForeignKey("dbo.DocumentLinks", "LinkTypeId", "dbo.DictionaryLinkTypes", "Id");
            AddForeignKey("dbo.DocumentLinks", "DocumentId", "dbo.Documents", "Id");
            AddForeignKey("dbo.DocumentSubscriptions", "DocumentEvents_Id1", "dbo.DocumentEvents", "Id");
            AddForeignKey("dbo.DocumentSubscriptions", "DocumentEvents_Id", "dbo.DocumentEvents", "Id");
            AddForeignKey("dbo.DocumentSubscriptions", "SendEventId", "dbo.DocumentEvents", "Id");
            AddForeignKey("dbo.DocumentSubscriptions", "DoneEventId", "dbo.DocumentEvents", "Id");
            AddForeignKey("dbo.DictionaryAgentPersons", "PersonAgentId", "dbo.DictionaryAgents", "Id");
            AddForeignKey("dbo.DictionaryAgentPersons", "AgentId", "dbo.DictionaryAgents", "Id");
            AddForeignKey("dbo.DictionaryDepartments", "DictionaryPositions_Id", "dbo.DictionaryPositions", "Id");
            AddForeignKey("dbo.DictionaryDepartments", "ChiefPositionId", "dbo.DictionaryPositions", "Id");
            AddForeignKey("dbo.DictionaryAgentPersons", "DictionaryAgents_Id", "dbo.DictionaryAgents", "Id");
        }
    }
}
