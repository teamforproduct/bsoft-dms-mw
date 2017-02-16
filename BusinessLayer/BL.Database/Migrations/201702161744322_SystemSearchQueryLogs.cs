namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SystemSearchQueryLogs : DbMigration
    {
        public override void Up()
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
                .PrimaryKey(t => t.Id)
                .Index(t => t.ClientId);
            
        }
        
        public override void Down()
        {
            DropIndex("DMS.SystemSearchQueryLogs", new[] { "ClientId" });
            DropTable("DMS.SystemSearchQueryLogs");
        }
    }
}
