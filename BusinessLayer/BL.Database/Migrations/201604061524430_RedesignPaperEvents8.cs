namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RedesignPaperEvents8 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("DMS.DocumentPapersTMPs", "DocumentId", "DMS.Documents");
            DropForeignKey("DMS.DocumentEvents", "PaperId", "DMS.DocumentPapersTMPs");
            DropForeignKey("DMS.DocumentPapersTMPs", "LastChangeUserId", "DMS.DocumentEvents");
            DropIndex("DMS.DocumentPapersTMPs", new[] { "DocumentId" });
            DropIndex("DMS.DocumentPapersTMPs", new[] { "LastChangeUserId" });
            DropTable("DMS.DocumentPaperEvents");
            DropTable("DMS.DocumentPapersTMPs");
        }
        
        public override void Down()
        {
            CreateTable(
                "DMS.DocumentPapersTMPs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        Name = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 2000),
                        IsMain = c.Boolean(nullable: false),
                        IsOriginal = c.Boolean(nullable: false),
                        IsCopy = c.Boolean(nullable: false),
                        PageQuantity = c.Int(nullable: false),
                        OrderNumber = c.Int(nullable: false),
                        IsInWork = c.Boolean(nullable: false),
                        LastPaperEventId = c.Int(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "DMS.DocumentPaperEvents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PaperId = c.Int(nullable: false),
                        EventTypeId = c.Int(nullable: false),
                        SendListId = c.Int(),
                        EventId = c.Int(),
                        Description = c.String(maxLength: 2000),
                        SourcePositionId = c.Int(),
                        SourcePositionExecutorAgentId = c.Int(),
                        SourceAgentId = c.Int(),
                        TargetPositionId = c.Int(),
                        TargetPositionExecutorAgentId = c.Int(),
                        TargetAgentId = c.Int(),
                        PaperListId = c.Int(),
                        PlanAgentId = c.Int(),
                        PlanDate = c.DateTime(),
                        SendAgentId = c.Int(),
                        SendDate = c.DateTime(),
                        RecieveAgentId = c.Int(),
                        RecieveDate = c.DateTime(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("DMS.DocumentPapersTMPs", "LastChangeUserId");
            CreateIndex("DMS.DocumentPapersTMPs", "DocumentId");
            AddForeignKey("DMS.DocumentPapersTMPs", "LastChangeUserId", "DMS.DocumentEvents", "Id");
            AddForeignKey("DMS.DocumentEvents", "PaperId", "DMS.DocumentPapersTMPs", "Id");
            AddForeignKey("DMS.DocumentPapersTMPs", "DocumentId", "DMS.Documents", "Id");
        }
    }
}
