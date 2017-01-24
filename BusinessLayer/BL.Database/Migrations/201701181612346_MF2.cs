namespace BL.Database.Migrations
{
    using DatabaseContext;
    using System;
    using System.Data.Entity.Migrations;

    public partial class MF2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("DMS.SystemPermissions", "ModuleFeatureId", "DMS.SystemModuleFetures");
            DropIndex("DMS.SystemPermissions", "IX_ModuleFeatureAccessType");
            CreateTable(
                "DMS.SystemFeatures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 2000),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "DMS.SystemModules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 2000),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("DMS.SystemPermissions", "ModuleId", c => c.Int(nullable: false));
            AddColumn("DMS.SystemPermissions", "FeatureId", c => c.Int(nullable: false));
            CreateIndex("DMS.SystemPermissions", new[] { "ModuleId", "FeatureId", "AccessTypeId" }, unique: true, name: "IX_ModuleFeatureAccessType");
            AddForeignKey("DMS.SystemPermissions", "FeatureId", "DMS.SystemFeatures", "Id");
            AddForeignKey("DMS.SystemPermissions", "ModuleId", "DMS.SystemModules", "Id");
            DropColumn("DMS.SystemPermissions", "ModuleFeatureId");
            DropTable("DMS.SystemModuleFetures");

        }
        
        public override void Down()
        {
            CreateTable(
                "DMS.SystemModuleFetures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Module = c.String(maxLength: 400),
                        Feature = c.String(maxLength: 400),
                        Name = c.String(maxLength: 2000),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("DMS.SystemPermissions", "ModuleFeatureId", c => c.Int(nullable: false));
            DropForeignKey("DMS.SystemPermissions", "ModuleId", "DMS.SystemModules");
            DropForeignKey("DMS.SystemPermissions", "FeatureId", "DMS.SystemFeatures");
            DropIndex("DMS.SystemPermissions", "IX_ModuleFeatureAccessType");
            DropColumn("DMS.SystemPermissions", "FeatureId");
            DropColumn("DMS.SystemPermissions", "ModuleId");
            DropTable("DMS.SystemModules");
            DropTable("DMS.SystemFeatures");
            CreateIndex("DMS.SystemPermissions", new[] { "ModuleFeatureId", "AccessTypeId" }, unique: true, name: "IX_ModuleFeatureAccessType");
            AddForeignKey("DMS.SystemPermissions", "ModuleFeatureId", "DMS.SystemModuleFetures", "Id");
        }
    }
}
