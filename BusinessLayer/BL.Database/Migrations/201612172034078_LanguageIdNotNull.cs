namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LanguageIdNotNull : DbMigration
    {
        public override void Up()
        {
            DropIndex("DMS.DictionaryAgentUsers", new[] { "LanguageId" });
            AlterColumn("DMS.DictionaryAgentUsers", "LanguageId", c => c.Int(nullable: false));
            CreateIndex("DMS.DictionaryAgentUsers", "LanguageId");
        }
        
        public override void Down()
        {
            DropIndex("DMS.DictionaryAgentUsers", new[] { "LanguageId" });
            AlterColumn("DMS.DictionaryAgentUsers", "LanguageId", c => c.Int());
            CreateIndex("DMS.DictionaryAgentUsers", "LanguageId");
        }
    }
}
