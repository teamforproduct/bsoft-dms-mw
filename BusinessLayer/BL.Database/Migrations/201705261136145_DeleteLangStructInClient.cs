namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteLangStructInClient : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("DMS.AdminLanguageValues", "LanguageId", "DMS.AdminLanguages");
            DropIndex("DMS.AdminLanguages", new[] { "Code" });
            DropIndex("DMS.AdminLanguages", new[] { "Name" });
            DropIndex("DMS.AdminLanguageValues", new[] { "ClientId" });
            DropIndex("DMS.AdminLanguageValues", "IX_Label");
            DropIndex("DMS.AdminLanguageValues", new[] { "LanguageId" });
            DropTable("DMS.AdminLanguages");
            DropTable("DMS.AdminLanguageValues");
        }
        
        public override void Down()
        {
            CreateTable(
                "DMS.AdminLanguageValues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        LanguageId = c.Int(nullable: false),
                        Label = c.String(maxLength: 400),
                        Value = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "DMS.AdminLanguages",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 400),
                        IsDefault = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("DMS.AdminLanguageValues", "LanguageId");
            CreateIndex("DMS.AdminLanguageValues", new[] { "Label", "LanguageId", "ClientId" }, unique: true, name: "IX_Label");
            CreateIndex("DMS.AdminLanguageValues", "ClientId");
            CreateIndex("DMS.AdminLanguages", "Name", unique: true);
            CreateIndex("DMS.AdminLanguages", "Code", unique: true);
            AddForeignKey("DMS.AdminLanguageValues", "LanguageId", "DMS.AdminLanguages", "Id");
        }
    }
}
