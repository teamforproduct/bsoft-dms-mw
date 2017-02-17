namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgentFavorites : DbMigration
    {
        public override void Up()
        {
            DropIndex("DMS.SystemSearchQueryLogs", new[] { "ClientId" });
            CreateTable(
                "DMS.DictionaryAgentFavorites",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AgentId = c.Int(nullable: false),
                        ObjectId = c.Int(nullable: false),
                        Module = c.String(maxLength: 200),
                        Feature = c.String(maxLength: 200),
                        Date = c.DateTime(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.AgentId)
                .Index(t => new { t.AgentId, t.ObjectId, t.Module, t.Feature }, unique: true, name: "IX_AgentObjectModuleFeature");
            
            DropTable("DMS.SystemSearchQueryLogs");
        }
        
        public override void Down()
        {
            CreateTable(
                "DMS.SystemSearchQueryLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        ModuleId = c.Int(nullable: false),
                        FeatureId = c.Int(nullable: false),
                        SearchQueryText = c.String(maxLength: 2000),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("DMS.DictionaryAgentFavorites", "AgentId", "DMS.DictionaryAgents");
            DropIndex("DMS.DictionaryAgentFavorites", "IX_AgentObjectModuleFeature");
            DropTable("DMS.DictionaryAgentFavorites");
            CreateIndex("DMS.SystemSearchQueryLogs", "ClientId");
        }
    }
}
