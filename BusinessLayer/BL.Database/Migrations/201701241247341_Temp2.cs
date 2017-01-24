namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Temp2 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "DMS.SystemPermissions2", newName: "SystemPermissions");
            RenameTable(name: "DMS.SystemAccessTypes2", newName: "SystemAccessTypes");
            RenameTable(name: "DMS.SystemFeatures2", newName: "SystemFeatures");
            RenameTable(name: "DMS.SystemModules2", newName: "SystemModules");
            CreateTable(
                "DMS.SystemAccessTypes2",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 2000),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true);
            
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.SystemModules2", t => t.ModuleId)
                .Index(t => new { t.ModuleId, t.Code }, unique: true, name: "IX_ModuleCode");
            
            CreateTable(
                "DMS.SystemModules2",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 2000),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true);
            
            CreateTable(
                "DMS.SystemPermissions2",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        ModuleId = c.Int(nullable: false),
                        FeatureId = c.Int(nullable: false),
                        AccessTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.SystemAccessTypes2", t => t.AccessTypeId)
                .ForeignKey("DMS.SystemFeatures2", t => t.FeatureId)
                .ForeignKey("DMS.SystemModules2", t => t.ModuleId)
                .Index(t => new { t.ModuleId, t.FeatureId, t.AccessTypeId }, unique: true, name: "IX_ModuleFeatureAccessType");
            
            AddForeignKey("DMS.AdminRolePermissions", "PermissionId", "DMS.SystemPermissions2", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.AdminRolePermissions", "PermissionId", "DMS.SystemPermissions2");
            DropForeignKey("DMS.SystemPermissions2", "ModuleId", "DMS.SystemModules2");
            DropForeignKey("DMS.SystemPermissions2", "FeatureId", "DMS.SystemFeatures2");
            DropForeignKey("DMS.SystemPermissions2", "AccessTypeId", "DMS.SystemAccessTypes2");
            DropForeignKey("DMS.SystemFeatures2", "ModuleId", "DMS.SystemModules2");
            DropIndex("DMS.SystemPermissions2", "IX_ModuleFeatureAccessType");
            DropIndex("DMS.SystemModules2", new[] { "Code" });
            DropIndex("DMS.SystemFeatures2", "IX_ModuleCode");
            DropIndex("DMS.SystemAccessTypes2", new[] { "Code" });
            DropTable("DMS.SystemPermissions2");
            DropTable("DMS.SystemModules2");
            DropTable("DMS.SystemFeatures2");
            DropTable("DMS.SystemAccessTypes2");
            RenameTable(name: "DMS.SystemModules", newName: "SystemModules2");
            RenameTable(name: "DMS.SystemFeatures", newName: "SystemFeatures2");
            RenameTable(name: "DMS.SystemAccessTypes", newName: "SystemAccessTypes2");
            RenameTable(name: "DMS.SystemPermissions", newName: "SystemPermissions2");
        }
    }
}
