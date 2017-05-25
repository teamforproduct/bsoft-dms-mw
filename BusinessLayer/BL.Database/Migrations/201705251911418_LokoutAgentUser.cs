namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LokoutAgentUser : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("DMS.DictionaryAgentUsers", "LanguageId", "DMS.AdminLanguages");
            DropIndex("DMS.DictionaryAgentUsers", new[] { "LanguageId" });
            AddColumn("DMS.DictionaryAgentUsers", "IsLockout", c => c.Boolean(nullable: false));
            DropColumn("DMS.DictionaryAgentUsers", "LanguageId");
            DropColumn("DMS.DictionaryAgentUsers", "IsSendEMail");
        }
        
        public override void Down()
        {
            AddColumn("DMS.DictionaryAgentUsers", "IsSendEMail", c => c.Boolean(nullable: false));
            AddColumn("DMS.DictionaryAgentUsers", "LanguageId", c => c.Int(nullable: false));
            DropColumn("DMS.DictionaryAgentUsers", "IsLockout");
            CreateIndex("DMS.DictionaryAgentUsers", "LanguageId");
            AddForeignKey("DMS.DictionaryAgentUsers", "LanguageId", "DMS.AdminLanguages", "Id");
        }
    }
}
