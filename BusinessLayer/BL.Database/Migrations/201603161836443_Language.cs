namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Language : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AdminLanguages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Name = c.String(),
                        IsDefault = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AdminLanguageValues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LanguageId = c.Int(nullable: false),
                        Label = c.String(),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AdminLanguages", t => t.LanguageId)
                .Index(t => t.LanguageId);
            
            AddColumn("dbo.DictionaryAgents", "LanguageId", c => c.Int());
            CreateIndex("dbo.DictionaryAgents", "LanguageId");
            AddForeignKey("dbo.DictionaryAgents", "LanguageId", "dbo.AdminLanguages", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DictionaryAgents", "LanguageId", "dbo.AdminLanguages");
            DropForeignKey("dbo.AdminLanguageValues", "LanguageId", "dbo.AdminLanguages");
            DropIndex("dbo.DictionaryAgents", new[] { "LanguageId" });
            DropIndex("dbo.AdminLanguageValues", new[] { "LanguageId" });
            DropColumn("dbo.DictionaryAgents", "LanguageId");
            DropTable("dbo.AdminLanguageValues");
            DropTable("dbo.AdminLanguages");
        }
    }
}
