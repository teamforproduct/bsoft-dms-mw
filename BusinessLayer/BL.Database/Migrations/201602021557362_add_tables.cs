namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_tables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DocumentSubscriptions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        SendEventId = c.Int(nullable: false),
                        DoneEventId = c.Int(),
                        Description = c.String(),
                        Hash = c.String(),
                        ChangedHash = c.String(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                        DocumentEvents_Id = c.Int(),
                        DocumentEvents_Id1 = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Documents", t => t.DocumentId)
                .ForeignKey("dbo.DocumentEvents", t => t.DoneEventId)
                .ForeignKey("dbo.DocumentEvents", t => t.SendEventId)
                .ForeignKey("dbo.DocumentEvents", t => t.DocumentEvents_Id)
                .ForeignKey("dbo.DocumentEvents", t => t.DocumentEvents_Id1)
                .Index(t => t.DocumentId)
                .Index(t => t.SendEventId)
                .Index(t => t.DoneEventId)
                .Index(t => t.DocumentEvents_Id)
                .Index(t => t.DocumentEvents_Id1);
            
            CreateTable(
                "dbo.DocumentWaits",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        ParentId = c.Int(),
                        OnEventId = c.Int(nullable: false),
                        OffEventId = c.Int(),
                        ResultTypeId = c.Int(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                        DocumentEvents_Id = c.Int(),
                        DocumentEvents_Id1 = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DocumentWaits", t => t.ParentId)
                .ForeignKey("dbo.Documents", t => t.DocumentId)
                .ForeignKey("dbo.DocumentEvents", t => t.OffEventId)
                .ForeignKey("dbo.DocumentEvents", t => t.OnEventId)
                .ForeignKey("dbo.DictionaryResultTypes", t => t.ResultTypeId)
                .ForeignKey("dbo.DocumentEvents", t => t.DocumentEvents_Id)
                .ForeignKey("dbo.DocumentEvents", t => t.DocumentEvents_Id1)
                .Index(t => t.DocumentId)
                .Index(t => t.ParentId)
                .Index(t => t.OnEventId)
                .Index(t => t.OffEventId)
                .Index(t => t.ResultTypeId)
                .Index(t => t.DocumentEvents_Id)
                .Index(t => t.DocumentEvents_Id1);
            
            CreateTable(
                "dbo.DictionaryResultTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsExecute = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Documents", t => t.DocumentId)
                .ForeignKey("dbo.DictionaryLinkTypes", t => t.LinkTypeId)
                .ForeignKey("dbo.Documents", t => t.ParentDocumentId)
                .ForeignKey("dbo.Documents", t => t.Documents_Id)
                .Index(t => t.DocumentId)
                .Index(t => t.ParentDocumentId)
                .Index(t => t.LinkTypeId)
                .Index(t => t.Documents_Id);
            
            CreateTable(
                "dbo.DictionaryLinkTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsImpotant = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.DocumentAccesses", "IsFavourtite", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DocumentLinks", "Documents_Id", "dbo.Documents");
            DropForeignKey("dbo.DocumentLinks", "ParentDocumentId", "dbo.Documents");
            DropForeignKey("dbo.DocumentLinks", "LinkTypeId", "dbo.DictionaryLinkTypes");
            DropForeignKey("dbo.DocumentLinks", "DocumentId", "dbo.Documents");
            DropForeignKey("dbo.DocumentSubscriptions", "DocumentEvents_Id1", "dbo.DocumentEvents");
            DropForeignKey("dbo.DocumentWaits", "DocumentEvents_Id1", "dbo.DocumentEvents");
            DropForeignKey("dbo.DocumentWaits", "DocumentEvents_Id", "dbo.DocumentEvents");
            DropForeignKey("dbo.DocumentWaits", "ResultTypeId", "dbo.DictionaryResultTypes");
            DropForeignKey("dbo.DocumentWaits", "OnEventId", "dbo.DocumentEvents");
            DropForeignKey("dbo.DocumentWaits", "OffEventId", "dbo.DocumentEvents");
            DropForeignKey("dbo.DocumentWaits", "DocumentId", "dbo.Documents");
            DropForeignKey("dbo.DocumentWaits", "ParentId", "dbo.DocumentWaits");
            DropForeignKey("dbo.DocumentSubscriptions", "DocumentEvents_Id", "dbo.DocumentEvents");
            DropForeignKey("dbo.DocumentSubscriptions", "SendEventId", "dbo.DocumentEvents");
            DropForeignKey("dbo.DocumentSubscriptions", "DoneEventId", "dbo.DocumentEvents");
            DropForeignKey("dbo.DocumentSubscriptions", "DocumentId", "dbo.Documents");
            DropIndex("dbo.DocumentLinks", new[] { "Documents_Id" });
            DropIndex("dbo.DocumentLinks", new[] { "LinkTypeId" });
            DropIndex("dbo.DocumentLinks", new[] { "ParentDocumentId" });
            DropIndex("dbo.DocumentLinks", new[] { "DocumentId" });
            DropIndex("dbo.DocumentWaits", new[] { "DocumentEvents_Id1" });
            DropIndex("dbo.DocumentWaits", new[] { "DocumentEvents_Id" });
            DropIndex("dbo.DocumentWaits", new[] { "ResultTypeId" });
            DropIndex("dbo.DocumentWaits", new[] { "OffEventId" });
            DropIndex("dbo.DocumentWaits", new[] { "OnEventId" });
            DropIndex("dbo.DocumentWaits", new[] { "ParentId" });
            DropIndex("dbo.DocumentWaits", new[] { "DocumentId" });
            DropIndex("dbo.DocumentSubscriptions", new[] { "DocumentEvents_Id1" });
            DropIndex("dbo.DocumentSubscriptions", new[] { "DocumentEvents_Id" });
            DropIndex("dbo.DocumentSubscriptions", new[] { "DoneEventId" });
            DropIndex("dbo.DocumentSubscriptions", new[] { "SendEventId" });
            DropIndex("dbo.DocumentSubscriptions", new[] { "DocumentId" });
            DropColumn("dbo.DocumentAccesses", "IsFavourtite");
            DropTable("dbo.DictionaryLinkTypes");
            DropTable("dbo.DocumentLinks");
            DropTable("dbo.DictionaryResultTypes");
            DropTable("dbo.DocumentWaits");
            DropTable("dbo.DocumentSubscriptions");
        }
    }
}
