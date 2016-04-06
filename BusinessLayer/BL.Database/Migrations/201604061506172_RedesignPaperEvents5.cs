namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RedesignPaperEvents5 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "DMS.DocumentPapers",
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.Documents", t => t.DocumentId)
                .ForeignKey("DMS.DocumentEvents", t => t.LastPaperEventId)
                .Index(t => t.DocumentId)
                .Index(t => t.LastPaperEventId);
            
            AddForeignKey("DMS.DocumentEvents", "PaperId", "DMS.DocumentPapers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.DocumentPapers", "LastPaperEventId", "DMS.DocumentEvents");
            DropForeignKey("DMS.DocumentEvents", "PaperId", "DMS.DocumentPapers");
            DropForeignKey("DMS.DocumentPapers", "DocumentId", "DMS.Documents");
            DropIndex("DMS.DocumentPapers", new[] { "LastPaperEventId" });
            DropIndex("DMS.DocumentPapers", new[] { "DocumentId" });
            DropTable("DMS.DocumentPapers");
        }
    }
}
