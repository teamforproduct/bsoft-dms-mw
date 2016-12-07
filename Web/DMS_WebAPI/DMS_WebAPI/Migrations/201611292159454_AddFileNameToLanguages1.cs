namespace DMS_WebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFileNameToLanguages1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AdminLanguageValues", "LanguageId", "dbo.AdminLanguages");
            DropIndex("dbo.AdminLanguageValues", new[] { "LanguageId" });
            AddColumn("dbo.AdminLanguages", "FileName", c => c.String(maxLength: 200));
            AlterColumn("dbo.AdminLanguages", "Code", c => c.String(maxLength: 200));
            AlterColumn("dbo.AdminLanguages", "Name", c => c.String(maxLength: 200));
            CreateIndex("dbo.AdminLanguages", "FileName");
            DropTable("dbo.AdminLanguageValues");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.AdminLanguageValues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LanguageId = c.Int(nullable: false),
                        Label = c.String(maxLength: 2000),
                        Value = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.Id);
            
            DropIndex("dbo.AdminLanguages", new[] { "FileName" });
            AlterColumn("dbo.AdminLanguages", "Name", c => c.String(maxLength: 2000));
            AlterColumn("dbo.AdminLanguages", "Code", c => c.String(maxLength: 2000));
            DropColumn("dbo.AdminLanguages", "FileName");
            CreateIndex("dbo.AdminLanguageValues", "LanguageId");
            AddForeignKey("dbo.AdminLanguageValues", "LanguageId", "dbo.AdminLanguages", "Id", cascadeDelete: true);
        }
    }
}
