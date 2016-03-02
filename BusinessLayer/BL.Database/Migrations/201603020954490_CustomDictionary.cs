namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomDictionary : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.DocumentSendLists", new[] { "SourcePositionId" });
            CreateTable(
                "dbo.CustomDictionaries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DictionaryTypeId = c.Int(nullable: false),
                        Code = c.String(),
                        Description = c.String(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CustomDictionaryTypes", t => t.DictionaryTypeId)
                .Index(t => t.DictionaryTypeId);
            
            CreateTable(
                "dbo.CustomDictionaryTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Description = c.String(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.DocumentEvents", "SendDate", c => c.DateTime());
            AlterColumn("dbo.DocumentSendLists", "SourcePositionId", c => c.Int(nullable: false));
            CreateIndex("dbo.DocumentSendLists", "SourcePositionId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CustomDictionaries", "DictionaryTypeId", "dbo.CustomDictionaryTypes");
            DropIndex("dbo.CustomDictionaries", new[] { "DictionaryTypeId" });
            DropIndex("dbo.DocumentSendLists", new[] { "SourcePositionId" });
            AlterColumn("dbo.DocumentSendLists", "SourcePositionId", c => c.Int());
            DropColumn("dbo.DocumentEvents", "SendDate");
            DropTable("dbo.CustomDictionaryTypes");
            DropTable("dbo.CustomDictionaries");
            CreateIndex("dbo.DocumentSendLists", "SourcePositionId");
        }
    }
}
