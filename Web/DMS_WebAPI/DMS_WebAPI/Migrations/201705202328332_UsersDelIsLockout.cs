namespace DMS_WebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UsersDelIsLockout : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetClients", "LanguageId", c => c.Int());
            AddColumn("dbo.AspNetUsers", "LastName", c => c.String(maxLength: 2000));
            AddColumn("dbo.AspNetUsers", "FirstName", c => c.String(maxLength: 2000));
            AddColumn("dbo.AspNetUsers", "LanguageId", c => c.Int());
            AlterColumn("dbo.AspNetUsers", "ControlAnswer", c => c.String(maxLength: 2000));
            CreateIndex("dbo.AspNetClients", "LanguageId");
            CreateIndex("dbo.AspNetUsers", "LanguageId");
            AddForeignKey("dbo.AspNetClients", "LanguageId", "dbo.AdminLanguages", "Id");
            AddForeignKey("dbo.AspNetUsers", "LanguageId", "dbo.AdminLanguages", "Id");
            DropColumn("dbo.AspNetUsers", "IsEmailConfirmRequired");
            DropColumn("dbo.AspNetUsers", "IsLockout");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "IsLockout", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "IsEmailConfirmRequired", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.AspNetUsers", "LanguageId", "dbo.AdminLanguages");
            DropForeignKey("dbo.AspNetClients", "LanguageId", "dbo.AdminLanguages");
            DropIndex("dbo.AspNetUsers", new[] { "LanguageId" });
            DropIndex("dbo.AspNetClients", new[] { "LanguageId" });
            AlterColumn("dbo.AspNetUsers", "ControlAnswer", c => c.String());
            DropColumn("dbo.AspNetUsers", "LanguageId");
            DropColumn("dbo.AspNetUsers", "FirstName");
            DropColumn("dbo.AspNetUsers", "LastName");
            DropColumn("dbo.AspNetClients", "LanguageId");
        }
    }
}
