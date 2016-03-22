namespace DMS_WebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddServersAndLanguages : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AdminLanguages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 2000),
                        Name = c.String(maxLength: 2000),
                        IsDefault = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AdminLanguageValues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LanguageId = c.Int(nullable: false),
                        Label = c.String(maxLength: 2000),
                        Value = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AdminLanguages", t => t.LanguageId, cascadeDelete: true)
                .Index(t => t.LanguageId);
            
            CreateTable(
                "dbo.AdminServers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Address = c.String(),
                        Name = c.String(),
                        ServerType = c.String(),
                        DefaultDatabase = c.String(),
                        IntegrateSecurity = c.Boolean(nullable: false),
                        UserName = c.String(),
                        UserPassword = c.String(),
                        ConnectionString = c.String(),
                        DefaultSchema = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AdminLanguageValues", "LanguageId", "dbo.AdminLanguages");
            DropIndex("dbo.AdminLanguageValues", new[] { "LanguageId" });
            DropTable("dbo.AdminServers");
            DropTable("dbo.AdminLanguageValues");
            DropTable("dbo.AdminLanguages");
        }
    }
}
