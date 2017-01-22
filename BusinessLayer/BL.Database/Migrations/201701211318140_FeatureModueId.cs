namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FeatureModueId : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.SystemFeatures", "ModuleId", c => c.Int(nullable: false));
            CreateIndex("DMS.SystemFeatures", "ModuleId");
            AddForeignKey("DMS.SystemFeatures", "ModuleId", "DMS.SystemModules", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.SystemFeatures", "ModuleId", "DMS.SystemModules");
            DropIndex("DMS.SystemFeatures", new[] { "ModuleId" });
            DropColumn("DMS.SystemFeatures", "ModuleId");
        }
    }
}
