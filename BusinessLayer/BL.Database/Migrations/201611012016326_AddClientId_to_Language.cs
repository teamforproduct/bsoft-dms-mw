namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddClientId_to_Language : DbMigration
    {
        public override void Up()
        {
            DropIndex("DMS.AdminLanguageValues", "IX_Label");
            AddColumn("DMS.AdminLanguageValues", "ClientId", c => c.Int(nullable: false));
            CreateIndex("DMS.AdminLanguageValues", "ClientId");
            CreateIndex("DMS.AdminLanguageValues", new[] { "Label", "LanguageId", "ClientId" }, unique: true, name: "IX_Label");
            CreateIndex("DMS.DictionaryAgentUsers", "LanguageId");
            AddForeignKey("DMS.DictionaryAgentUsers", "LanguageId", "DMS.AdminLanguages", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.DictionaryAgentUsers", "LanguageId", "DMS.AdminLanguages");
            DropIndex("DMS.DictionaryAgentUsers", new[] { "LanguageId" });
            DropIndex("DMS.AdminLanguageValues", "IX_Label");
            DropIndex("DMS.AdminLanguageValues", new[] { "ClientId" });
            DropColumn("DMS.AdminLanguageValues", "ClientId");
            CreateIndex("DMS.AdminLanguageValues", new[] { "Label", "LanguageId" }, unique: true, name: "IX_Label");
        }
    }
}
