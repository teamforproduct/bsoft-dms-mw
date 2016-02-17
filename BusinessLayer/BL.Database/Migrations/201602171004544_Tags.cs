namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Tags : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DictionaryTags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        PositionId = c.Int(),
                        Color = c.String(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DictionaryPositions", t => t.PositionId)
                .Index(t => t.PositionId);
            
            CreateTable(
                "dbo.DocumentTags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        TagId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Documents", t => t.DocumentId)
                .ForeignKey("dbo.DictionaryTags", t => t.TagId)
                .Index(t => t.DocumentId)
                .Index(t => t.TagId);
            
            AddColumn("dbo.DocumentEvents", "ReadAgentId", c => c.Int());
            CreateIndex("dbo.DocumentEvents", "ReadAgentId");
            AddForeignKey("dbo.DocumentEvents", "ReadAgentId", "dbo.DictionaryAgents", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DictionaryTags", "PositionId", "dbo.DictionaryPositions");
            DropForeignKey("dbo.DocumentTags", "TagId", "dbo.DictionaryTags");
            DropForeignKey("dbo.DocumentTags", "DocumentId", "dbo.Documents");
            DropForeignKey("dbo.DocumentEvents", "ReadAgentId", "dbo.DictionaryAgents");
            DropIndex("dbo.DocumentEvents", new[] { "ReadAgentId" });
            DropIndex("dbo.DocumentTags", new[] { "TagId" });
            DropIndex("dbo.DocumentTags", new[] { "DocumentId" });
            DropIndex("dbo.DictionaryTags", new[] { "PositionId" });
            DropColumn("dbo.DocumentEvents", "ReadAgentId");
            DropTable("dbo.DocumentTags");
            DropTable("dbo.DictionaryTags");
        }
    }
}
