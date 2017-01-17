namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModuleFeature2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("DMS.SystemPermissions", "FeatureId", "DMS.SystemFeatures");
            DropForeignKey("DMS.SystemPermissions", "ModuleId", "DMS.SystemModules");
            DropIndex("DMS.SystemActions", "IX_ObjectCode");
            DropIndex("DMS.SystemPermissions", "IX_ModuleFeatureAccessType");
            DropIndex("DMS.SystemFeatures", new[] { "Code" });
            DropIndex("DMS.SystemModules", new[] { "Code" });
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
            AlterColumn("DMS.SystemActions", "PermissionId", c => c.Int(nullable: false));
            CreateIndex("DMS.SystemActions", new[] { "PermissionId", "Code" }, unique: true, name: "IX_ObjectCode");
            CreateIndex("DMS.SystemPermissions", new[] { "ModuleFeatureId", "AccessTypeId" }, unique: true, name: "IX_ModuleFeatureAccessType");
            AddForeignKey("DMS.SystemPermissions", "ModuleFeatureId", "DMS.SystemModuleFetures", "Id");
            DropColumn("DMS.SystemPermissions", "ModuleId");
            DropColumn("DMS.SystemPermissions", "FeatureId");
            DropTable("DMS.SystemFeatures");
            DropTable("DMS.SystemModules");
        }
        
        public override void Down()
        {
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
            
            AddColumn("DMS.SystemPermissions", "FeatureId", c => c.Int(nullable: false));
            AddColumn("DMS.SystemPermissions", "ModuleId", c => c.Int(nullable: false));
            DropForeignKey("DMS.SystemPermissions", "ModuleFeatureId", "DMS.SystemModuleFetures");
            DropIndex("DMS.SystemPermissions", "IX_ModuleFeatureAccessType");
            DropIndex("DMS.SystemActions", "IX_ObjectCode");
            AlterColumn("DMS.SystemActions", "PermissionId", c => c.Int());
            DropColumn("DMS.SystemPermissions", "ModuleFeatureId");
            DropTable("DMS.SystemModuleFetures");
            CreateIndex("DMS.SystemModules", "Code", unique: true);
            CreateIndex("DMS.SystemFeatures", "Code", unique: true);
            CreateIndex("DMS.SystemPermissions", new[] { "ModuleId", "FeatureId", "AccessTypeId" }, unique: true, name: "IX_ModuleFeatureAccessType");
            CreateIndex("DMS.SystemActions", new[] { "PermissionId", "Code" }, unique: true, name: "IX_ObjectCode");
            AddForeignKey("DMS.SystemPermissions", "ModuleId", "DMS.SystemModules", "Id");
            AddForeignKey("DMS.SystemPermissions", "FeatureId", "DMS.SystemFeatures", "Id");
        }
    }
}
