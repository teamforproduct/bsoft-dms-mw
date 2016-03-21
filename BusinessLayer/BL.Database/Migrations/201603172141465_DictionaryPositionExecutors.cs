namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DictionaryPositionExecutors : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AdminPositionRoles", "AccessLevelId", "dbo.AdminAccessLevels");
            DropIndex("dbo.AdminPositionRoles", new[] { "AccessLevelId" });
            CreateTable(
                "dbo.DictionaryPositionExecutors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AgentId = c.Int(nullable: false),
                        PositionId = c.Int(nullable: false),
                        PositionExecutorTypeId = c.Int(nullable: false),
                        AccessLevelId = c.Int(nullable: false),
                        Description = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AdminAccessLevels", t => t.AccessLevelId)
                .ForeignKey("dbo.DictionaryAgents", t => t.AgentId)
                .ForeignKey("dbo.DictionaryPositions", t => t.PositionId)
                .ForeignKey("dbo.DictionaryPositionExecutorTypes", t => t.PositionExecutorTypeId)
                .Index(t => t.AgentId)
                .Index(t => t.PositionId)
                .Index(t => t.PositionExecutorTypeId)
                .Index(t => t.AccessLevelId);
            
            CreateTable(
                "dbo.DictionaryPositionExecutorTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Name = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DocumentTasks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        PositionId = c.Int(),
                        Task = c.String(),
                        Description = c.String(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Documents", t => t.DocumentId)
                .ForeignKey("dbo.DictionaryPositions", t => t.PositionId)
                .Index(t => t.DocumentId)
                .Index(t => t.PositionId);
            
            CreateTable(
                "dbo.DocumentEventReaders",
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DictionaryAgents", t => t.AgentId)
                .ForeignKey("dbo.DocumentEvents", t => t.EventId)
                .ForeignKey("dbo.DictionaryPositions", t => t.PositionId)
                .ForeignKey("dbo.DictionaryAgents", t => t.ReadAgentId)
                .Index(t => t.EventId)
                .Index(t => t.PositionId)
                .Index(t => t.AgentId)
                .Index(t => t.ReadAgentId);
            
            AddColumn("dbo.DocumentEvents", "TaskId", c => c.Int());
            AddColumn("dbo.DocumentEvents", "IsAvailableWithinTask", c => c.Boolean(nullable: false));
            AddColumn("dbo.DocumentSendLists", "TaskId", c => c.Int());
            AddColumn("dbo.DocumentSendLists", "IsAddControl", c => c.Boolean(nullable: false));
            AddColumn("dbo.DocumentSendLists", "IsAvailableWithinTask", c => c.Boolean(nullable: false));
            AddColumn("dbo.TemplateDocumentSendListsSet", "IsAddControl", c => c.Boolean(nullable: false));
            AddColumn("dbo.TemplateDocumentSendListsSet", "IsAvailableWithinTask", c => c.Boolean(nullable: false));
            CreateIndex("dbo.DocumentEvents", "TaskId");
            CreateIndex("dbo.DocumentSendLists", "TaskId");
            AddForeignKey("dbo.DocumentEvents", "TaskId", "dbo.DocumentTasks", "Id");
            AddForeignKey("dbo.DocumentSendLists", "TaskId", "dbo.DocumentTasks", "Id");
            DropColumn("dbo.AdminPositionRoles", "AccessLevelId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AdminPositionRoles", "AccessLevelId", c => c.Int(nullable: false));
            DropForeignKey("dbo.DocumentEventReaders", "ReadAgentId", "dbo.DictionaryAgents");
            DropForeignKey("dbo.DocumentEventReaders", "PositionId", "dbo.DictionaryPositions");
            DropForeignKey("dbo.DocumentEventReaders", "EventId", "dbo.DocumentEvents");
            DropForeignKey("dbo.DocumentEventReaders", "AgentId", "dbo.DictionaryAgents");
            DropForeignKey("dbo.DocumentSendLists", "TaskId", "dbo.DocumentTasks");
            DropForeignKey("dbo.DocumentTasks", "PositionId", "dbo.DictionaryPositions");
            DropForeignKey("dbo.DocumentEvents", "TaskId", "dbo.DocumentTasks");
            DropForeignKey("dbo.DocumentTasks", "DocumentId", "dbo.Documents");
            DropForeignKey("dbo.DictionaryPositionExecutors", "PositionExecutorTypeId", "dbo.DictionaryPositionExecutorTypes");
            DropForeignKey("dbo.DictionaryPositionExecutors", "PositionId", "dbo.DictionaryPositions");
            DropForeignKey("dbo.DictionaryPositionExecutors", "AgentId", "dbo.DictionaryAgents");
            DropForeignKey("dbo.DictionaryPositionExecutors", "AccessLevelId", "dbo.AdminAccessLevels");
            DropIndex("dbo.DocumentEventReaders", new[] { "ReadAgentId" });
            DropIndex("dbo.DocumentEventReaders", new[] { "AgentId" });
            DropIndex("dbo.DocumentEventReaders", new[] { "PositionId" });
            DropIndex("dbo.DocumentEventReaders", new[] { "EventId" });
            DropIndex("dbo.DocumentTasks", new[] { "PositionId" });
            DropIndex("dbo.DocumentTasks", new[] { "DocumentId" });
            DropIndex("dbo.DocumentSendLists", new[] { "TaskId" });
            DropIndex("dbo.DocumentEvents", new[] { "TaskId" });
            DropIndex("dbo.DictionaryPositionExecutors", new[] { "AccessLevelId" });
            DropIndex("dbo.DictionaryPositionExecutors", new[] { "PositionExecutorTypeId" });
            DropIndex("dbo.DictionaryPositionExecutors", new[] { "PositionId" });
            DropIndex("dbo.DictionaryPositionExecutors", new[] { "AgentId" });
            DropColumn("dbo.TemplateDocumentSendListsSet", "IsAvailableWithinTask");
            DropColumn("dbo.TemplateDocumentSendListsSet", "IsAddControl");
            DropColumn("dbo.DocumentSendLists", "IsAvailableWithinTask");
            DropColumn("dbo.DocumentSendLists", "IsAddControl");
            DropColumn("dbo.DocumentSendLists", "TaskId");
            DropColumn("dbo.DocumentEvents", "IsAvailableWithinTask");
            DropColumn("dbo.DocumentEvents", "TaskId");
            DropTable("dbo.DocumentEventReaders");
            DropTable("dbo.DocumentTasks");
            DropTable("dbo.DictionaryPositionExecutorTypes");
            DropTable("dbo.DictionaryPositionExecutors");
            CreateIndex("dbo.AdminPositionRoles", "AccessLevelId");
            AddForeignKey("dbo.AdminPositionRoles", "AccessLevelId", "dbo.AdminAccessLevels", "Id");
        }
    }
}
