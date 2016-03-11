namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Contacts : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.DictionaryPhoneNumberTypes", newName: "DictionaryContactTypes");
            DropForeignKey("dbo.DictionaryAgentPhoneNumbers", "AgentId", "dbo.DictionaryAgents");
            DropForeignKey("dbo.DictionaryAgentPhoneNumbers", "PhoneTypeId", "dbo.DictionaryPhoneNumberTypes");
            DropIndex("dbo.DictionaryAgentPhoneNumbers", new[] { "AgentId" });
            DropIndex("dbo.DictionaryAgentPhoneNumbers", new[] { "PhoneTypeId" });
            CreateTable(
                "dbo.DictionaryAgentContacts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AgentId = c.Int(nullable: false),
                        ContactTypeId = c.Int(nullable: false),
                        Contact = c.String(),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DictionaryAgents", t => t.AgentId)
                .ForeignKey("dbo.DictionaryContactTypes", t => t.ContactTypeId)
                .Index(t => t.AgentId)
                .Index(t => t.ContactTypeId);
            
            AddColumn("dbo.DictionaryContactTypes", "Code", c => c.String());
            AddColumn("dbo.DictionaryContactTypes", "InputMask", c => c.String());
            AddColumn("dbo.Documents", "ExecutorPositionExecutorAgentId", c => c.Int(nullable: false));
            AddColumn("dbo.DocumentEvents", "SourcePositionExecutorAgentId", c => c.Int());
            AddColumn("dbo.DocumentEvents", "TargetPositionExecutorAgentId", c => c.Int());
            AddColumn("dbo.DocumentSendLists", "SourcePositionExecutorAgentId", c => c.Int(nullable: false));
            AddColumn("dbo.DocumentSendLists", "TargetPositionExecutorAgentId", c => c.Int());
            DropTable("dbo.DictionaryAgentPhoneNumbers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.DictionaryAgentPhoneNumbers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AgentId = c.Int(nullable: false),
                        PhoneTypeId = c.Int(nullable: false),
                        PhoneNumber = c.String(),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.DictionaryAgentContacts", "ContactTypeId", "dbo.DictionaryContactTypes");
            DropForeignKey("dbo.DictionaryAgentContacts", "AgentId", "dbo.DictionaryAgents");
            DropIndex("dbo.DictionaryAgentContacts", new[] { "ContactTypeId" });
            DropIndex("dbo.DictionaryAgentContacts", new[] { "AgentId" });
            DropColumn("dbo.DocumentSendLists", "TargetPositionExecutorAgentId");
            DropColumn("dbo.DocumentSendLists", "SourcePositionExecutorAgentId");
            DropColumn("dbo.DocumentEvents", "TargetPositionExecutorAgentId");
            DropColumn("dbo.DocumentEvents", "SourcePositionExecutorAgentId");
            DropColumn("dbo.Documents", "ExecutorPositionExecutorAgentId");
            DropColumn("dbo.DictionaryContactTypes", "InputMask");
            DropColumn("dbo.DictionaryContactTypes", "Code");
            DropTable("dbo.DictionaryAgentContacts");
            CreateIndex("dbo.DictionaryAgentPhoneNumbers", "PhoneTypeId");
            CreateIndex("dbo.DictionaryAgentPhoneNumbers", "AgentId");
            AddForeignKey("dbo.DictionaryAgentPhoneNumbers", "PhoneTypeId", "dbo.DictionaryPhoneNumberTypes", "Id");
            AddForeignKey("dbo.DictionaryAgentPhoneNumbers", "AgentId", "dbo.DictionaryAgents", "Id");
            RenameTable(name: "dbo.DictionaryContactTypes", newName: "DictionaryPhoneNumberTypes");
        }
    }
}
