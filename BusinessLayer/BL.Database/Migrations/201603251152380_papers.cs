namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class papers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "DMS.DocumentPaperEvents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PaperId = c.Int(nullable: false),
                        EventTypeId = c.Int(nullable: false),
                        EventId = c.Int(),
                        Description = c.String(),
                        SourcePositionId = c.Int(),
                        SourcePositionExecutorAgentId = c.Int(),
                        SourceAgentId = c.Int(),
                        TargetPositionId = c.Int(),
                        TargetPositionExecutorAgentId = c.Int(),
                        TargetAgentId = c.Int(),
                        PaperListId = c.Int(),
                        PlanAgentId = c.Int(nullable: false),
                        PlanDate = c.DateTime(nullable: false),
                        SendAgentId = c.Int(),
                        SendDate = c.DateTime(),
                        RecieveAgentId = c.Int(),
                        RecieveDate = c.DateTime(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DocumentEvents", t => t.EventId)
                .ForeignKey("DMS.DictionaryEventTypes", t => t.EventTypeId)
                .ForeignKey("DMS.DocumentPapers", t => t.PaperId)
                .ForeignKey("DMS.DocumentPaperLists", t => t.PaperListId)
                .ForeignKey("DMS.DictionaryAgents", t => t.PlanAgentId)
                .ForeignKey("DMS.DictionaryAgents", t => t.RecieveAgentId)
                .ForeignKey("DMS.DictionaryAgents", t => t.SendAgentId)
                .ForeignKey("DMS.DictionaryAgents", t => t.SourceAgentId)
                .ForeignKey("DMS.DictionaryPositions", t => t.SourcePositionId)
                .ForeignKey("DMS.DictionaryAgents", t => t.SourcePositionExecutorAgentId)
                .ForeignKey("DMS.DictionaryAgents", t => t.TargetAgentId)
                .ForeignKey("DMS.DictionaryPositions", t => t.TargetPositionId)
                .ForeignKey("DMS.DictionaryAgents", t => t.TargetPositionExecutorAgentId)
                .Index(t => t.PaperId)
                .Index(t => t.EventTypeId)
                .Index(t => t.EventId)
                .Index(t => t.SourcePositionId)
                .Index(t => t.SourcePositionExecutorAgentId)
                .Index(t => t.SourceAgentId)
                .Index(t => t.TargetPositionId)
                .Index(t => t.TargetPositionExecutorAgentId)
                .Index(t => t.TargetAgentId)
                .Index(t => t.PaperListId)
                .Index(t => t.PlanAgentId)
                .Index(t => t.SendAgentId)
                .Index(t => t.RecieveAgentId);
            
            CreateTable(
                "DMS.DocumentPapers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        IsMain = c.Boolean(nullable: false),
                        IsOriginal = c.Boolean(nullable: false),
                        IsCopy = c.Boolean(nullable: false),
                        PageQuantity = c.Int(nullable: false),
                        OrderNumber = c.Int(nullable: false),
                        LastPaperEventId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.Documents", t => t.DocumentId)
                .Index(t => t.DocumentId);
            
            CreateTable(
                "DMS.DocumentPaperLists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Description = c.String(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.DocumentPaperEvents", "TargetPositionExecutorAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentPaperEvents", "TargetPositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentPaperEvents", "TargetAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentPaperEvents", "SourcePositionExecutorAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentPaperEvents", "SourcePositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentPaperEvents", "SourceAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentPaperEvents", "SendAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentPaperEvents", "RecieveAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentPaperEvents", "PlanAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentPaperEvents", "PaperListId", "DMS.DocumentPaperLists");
            DropForeignKey("DMS.DocumentPaperEvents", "PaperId", "DMS.DocumentPapers");
            DropForeignKey("DMS.DocumentPapers", "DocumentId", "DMS.Documents");
            DropForeignKey("DMS.DocumentPaperEvents", "EventTypeId", "DMS.DictionaryEventTypes");
            DropForeignKey("DMS.DocumentPaperEvents", "EventId", "DMS.DocumentEvents");
            DropIndex("DMS.DocumentPapers", new[] { "DocumentId" });
            DropIndex("DMS.DocumentPaperEvents", new[] { "RecieveAgentId" });
            DropIndex("DMS.DocumentPaperEvents", new[] { "SendAgentId" });
            DropIndex("DMS.DocumentPaperEvents", new[] { "PlanAgentId" });
            DropIndex("DMS.DocumentPaperEvents", new[] { "PaperListId" });
            DropIndex("DMS.DocumentPaperEvents", new[] { "TargetAgentId" });
            DropIndex("DMS.DocumentPaperEvents", new[] { "TargetPositionExecutorAgentId" });
            DropIndex("DMS.DocumentPaperEvents", new[] { "TargetPositionId" });
            DropIndex("DMS.DocumentPaperEvents", new[] { "SourceAgentId" });
            DropIndex("DMS.DocumentPaperEvents", new[] { "SourcePositionExecutorAgentId" });
            DropIndex("DMS.DocumentPaperEvents", new[] { "SourcePositionId" });
            DropIndex("DMS.DocumentPaperEvents", new[] { "EventId" });
            DropIndex("DMS.DocumentPaperEvents", new[] { "EventTypeId" });
            DropIndex("DMS.DocumentPaperEvents", new[] { "PaperId" });
            DropTable("DMS.DocumentPaperLists");
            DropTable("DMS.DocumentPapers");
            DropTable("DMS.DocumentPaperEvents");
        }
    }
}
