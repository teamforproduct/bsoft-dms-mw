namespace DMS_WebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UsersDelIsLockout2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetClients", "LanguageId", "dbo.AdminLanguages");
            DropForeignKey("dbo.AspNetUsers", "LanguageId", "dbo.AdminLanguages");
            DropIndex("dbo.AspNetClients", new[] { "LanguageId" });
            DropIndex("dbo.AspNetUsers", new[] { "LanguageId" });
            AlterColumn("dbo.AspNetClients", "LanguageId", c => c.Int(nullable: false));
            AlterColumn("dbo.AspNetUsers", "LanguageId", c => c.Int(nullable: false));
            CreateIndex("dbo.AspNetClients", "LanguageId");
            CreateIndex("dbo.AspNetUsers", "LanguageId");
            AddForeignKey("dbo.AspNetClients", "LanguageId", "dbo.AdminLanguages", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AspNetUsers", "LanguageId", "dbo.AdminLanguages", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "LanguageId", "dbo.AdminLanguages");
            DropForeignKey("dbo.AspNetClients", "LanguageId", "dbo.AdminLanguages");
            DropIndex("dbo.AspNetUsers", new[] { "LanguageId" });
            DropIndex("dbo.AspNetClients", new[] { "LanguageId" });
            AlterColumn("dbo.AspNetUsers", "LanguageId", c => c.Int());
            AlterColumn("dbo.AspNetClients", "LanguageId", c => c.Int());
            CreateIndex("dbo.AspNetUsers", "LanguageId");
            CreateIndex("dbo.AspNetClients", "LanguageId");
            AddForeignKey("dbo.AspNetUsers", "LanguageId", "dbo.AdminLanguages", "Id");
            AddForeignKey("dbo.AspNetClients", "LanguageId", "dbo.AdminLanguages", "Id");
        }
    }
}
