namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DelPermissionsAll : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("DMS.SystemPermissions", "AccessTypeId", "DMS.SystemAccessTypes");
            DropForeignKey("DMS.SystemFeatures", "ModuleId", "DMS.SystemModules");
            DropForeignKey("DMS.SystemPermissions", "FeatureId", "DMS.SystemFeatures");
            DropForeignKey("DMS.SystemPermissions", "ModuleId", "DMS.SystemModules");
            DropForeignKey("DMS.AdminRolePermissions", "PermissionId", "DMS.SystemPermissions");
            DropForeignKey("DMS.SystemActions", "PermissionId", "DMS.SystemPermissions");
            DropForeignKey("DMS.SystemFeatures2", "ModuleId", "DMS.SystemModules2");
            DropForeignKey("DMS.SystemPermissions2", "AccessTypeId", "DMS.SystemAccessTypes2");
            DropForeignKey("DMS.SystemPermissions2", "FeatureId", "DMS.SystemFeatures2");
            DropForeignKey("DMS.SystemPermissions2", "ModuleId", "DMS.SystemModules2");
            DropForeignKey("DMS.AdminRolePermissions", "PermissionId", "DMS.SystemPermissions2");
            DropIndex("DMS.SystemActions", new[] { "PermissionId" });
            DropIndex("DMS.SystemPermissions", "IX_ModuleFeatureAccessType");
            DropIndex("DMS.SystemAccessTypes", new[] { "Code" });
            DropIndex("DMS.SystemFeatures", "IX_ModuleCode");
            DropIndex("DMS.SystemModules", new[] { "Code" });
            DropIndex("DMS.SystemAccessTypes2", new[] { "Code" });
            DropIndex("DMS.SystemFeatures2", "IX_ModuleCode");
            DropIndex("DMS.SystemModules2", new[] { "Code" });
            DropIndex("DMS.SystemPermissions2", "IX_ModuleFeatureAccessType");
            DropTable("DMS.SystemPermissions");
            DropTable("DMS.SystemAccessTypes");
            DropTable("DMS.SystemFeatures");
            DropTable("DMS.SystemModules");
            DropTable("DMS.SystemAccessTypes2");
            DropTable("DMS.SystemFeatures2");
            DropTable("DMS.SystemModules2");
            DropTable("DMS.SystemPermissions2");
        }
        
        public override void Down()
        {
            CreateTable(
                "DMS.SystemPermissions2",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        ModuleId = c.Int(nullable: false),
                        FeatureId = c.Int(nullable: false),
                        AccessTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "DMS.SystemModules2",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 2000),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "DMS.SystemFeatures2",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        ModuleId = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 2000),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "DMS.SystemAccessTypes2",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 2000),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "DMS.SystemModules",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 2000),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "DMS.SystemFeatures",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        ModuleId = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 2000),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "DMS.SystemAccessTypes",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 2000),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "DMS.SystemPermissions",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        ModuleId = c.Int(nullable: false),
                        FeatureId = c.Int(nullable: false),
                        AccessTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("DMS.SystemPermissions2", new[] { "ModuleId", "FeatureId", "AccessTypeId" }, unique: true, name: "IX_ModuleFeatureAccessType");
            CreateIndex("DMS.SystemModules2", "Code", unique: true);
            CreateIndex("DMS.SystemFeatures2", new[] { "ModuleId", "Code" }, unique: true, name: "IX_ModuleCode");
            CreateIndex("DMS.SystemAccessTypes2", "Code", unique: true);
            CreateIndex("DMS.SystemModules", "Code", unique: true);
            CreateIndex("DMS.SystemFeatures", new[] { "ModuleId", "Code" }, unique: true, name: "IX_ModuleCode");
            CreateIndex("DMS.SystemAccessTypes", "Code", unique: true);
            CreateIndex("DMS.SystemPermissions", new[] { "ModuleId", "FeatureId", "AccessTypeId" }, unique: true, name: "IX_ModuleFeatureAccessType");
            CreateIndex("DMS.SystemActions", "PermissionId");
            AddForeignKey("DMS.AdminRolePermissions", "PermissionId", "DMS.SystemPermissions2", "Id");
            AddForeignKey("DMS.SystemPermissions2", "ModuleId", "DMS.SystemModules2", "Id");
            AddForeignKey("DMS.SystemPermissions2", "FeatureId", "DMS.SystemFeatures2", "Id");
            AddForeignKey("DMS.SystemPermissions2", "AccessTypeId", "DMS.SystemAccessTypes2", "Id");
            AddForeignKey("DMS.SystemFeatures2", "ModuleId", "DMS.SystemModules2", "Id");
            AddForeignKey("DMS.SystemActions", "PermissionId", "DMS.SystemPermissions", "Id");
            AddForeignKey("DMS.AdminRolePermissions", "PermissionId", "DMS.SystemPermissions", "Id");
            AddForeignKey("DMS.SystemPermissions", "ModuleId", "DMS.SystemModules", "Id");
            AddForeignKey("DMS.SystemPermissions", "FeatureId", "DMS.SystemFeatures", "Id");
            AddForeignKey("DMS.SystemFeatures", "ModuleId", "DMS.SystemModules", "Id");
            AddForeignKey("DMS.SystemPermissions", "AccessTypeId", "DMS.SystemAccessTypes", "Id");
        }
    }
}
