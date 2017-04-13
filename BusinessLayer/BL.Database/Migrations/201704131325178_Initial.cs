namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("DMS.DictionaryAgentUsers", "LanguageId", "DMS.AdminLanguages");
            DropForeignKey("DMS.AdminLanguageValues", "LanguageId", "DMS.AdminLanguages");
            DropPrimaryKey("DMS.AdminLanguages");
            AlterColumn("DMS.AdminLanguages", "Id", c => c.Int(nullable: false));
            AddPrimaryKey("DMS.AdminLanguages", "Id");
            AddForeignKey("DMS.DictionaryAgentUsers", "LanguageId", "DMS.AdminLanguages", "Id");
            AddForeignKey("DMS.AdminLanguageValues", "LanguageId", "DMS.AdminLanguages", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.AdminLanguageValues", "LanguageId", "DMS.AdminLanguages");
            DropForeignKey("DMS.DictionaryAgentUsers", "LanguageId", "DMS.AdminLanguages");
            DropPrimaryKey("DMS.AdminLanguages");
            AlterColumn("DMS.AdminLanguages", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("DMS.AdminLanguages", "Id");
            AddForeignKey("DMS.AdminLanguageValues", "LanguageId", "DMS.AdminLanguages", "Id");
            AddForeignKey("DMS.DictionaryAgentUsers", "LanguageId", "DMS.AdminLanguages", "Id");
        }
    }
}
