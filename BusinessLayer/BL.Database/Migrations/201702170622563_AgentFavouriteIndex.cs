namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgentFavouriteIndex : DbMigration
    {
        public override void Up()
        {
            CreateIndex("DMS.DictionaryAgentFavorites", "AgentId", name: "IX_Agent");
            CreateIndex("DMS.DictionaryAgentFavorites", "Module");
            CreateIndex("DMS.DictionaryAgentFavorites", "Feature");
        }
        
        public override void Down()
        {
            DropIndex("DMS.DictionaryAgentFavorites", new[] { "Feature" });
            DropIndex("DMS.DictionaryAgentFavorites", new[] { "Module" });
            DropIndex("DMS.DictionaryAgentFavorites", "IX_Agent");
        }
    }
}
