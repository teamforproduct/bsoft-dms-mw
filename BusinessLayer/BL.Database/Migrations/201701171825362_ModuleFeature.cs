namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModuleFeature : DbMigration
    {
        public override void Up()
        {
            DropIndex("DMS.SystemActions", "IX_ObjectCode");
            CreateTable(
                "DMS.SystemPermissions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ModuleId = c.Int(nullable: false),
                        FeatureId = c.Int(nullable: false),
                        AccessTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.SystemAccessTypes", t => t.AccessTypeId)
                .ForeignKey("DMS.SystemFeatures", t => t.FeatureId)
                .ForeignKey("DMS.SystemModules", t => t.ModuleId)
                .Index(t => new { t.ModuleId, t.FeatureId, t.AccessTypeId }, unique: true, name: "IX_ModuleFeatureAccessType");
            
            CreateTable(
                "DMS.SystemAccessTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 2000),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true);
            
            CreateTable(
                "DMS.SystemFeatures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 2000),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true);
            
            CreateTable(
                "DMS.SystemModules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 2000),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true);
            
            CreateTable(
                "DMS.AdminRolePermissions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoleId = c.Int(nullable: false),
                        PermissionId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.SystemPermissions", t => t.PermissionId)
                .ForeignKey("DMS.AdminRoles", t => t.RoleId)
                .Index(t => new { t.PermissionId, t.RoleId }, unique: true, name: "IX_PermissionRole");
            
            AddColumn("DMS.SystemActions", "PermissionId", c => c.Int());
            CreateIndex("DMS.SystemActions", new[] { "PermissionId", "Code" }, unique: true, name: "IX_ObjectCode");
            CreateIndex("DMS.SystemActions", "ObjectId");
            AddForeignKey("DMS.SystemActions", "PermissionId", "DMS.SystemPermissions", "Id");
            DropColumn("DMS.SystemActions", "Module");
            DropColumn("DMS.SystemActions", "Feature");
            DropColumn("DMS.SystemActions", "CRUR");
        }
        
        public override void Down()
        {
            AddColumn("DMS.SystemActions", "CRUR", c => c.String(maxLength: 2000));
            AddColumn("DMS.SystemActions", "Feature", c => c.String(maxLength: 2000));
            AddColumn("DMS.SystemActions", "Module", c => c.String(maxLength: 2000));
            DropForeignKey("DMS.AdminRolePermissions", "RoleId", "DMS.AdminRoles");
            DropForeignKey("DMS.AdminRolePermissions", "PermissionId", "DMS.SystemPermissions");
            DropForeignKey("DMS.SystemActions", "PermissionId", "DMS.SystemPermissions");
            DropForeignKey("DMS.SystemPermissions", "ModuleId", "DMS.SystemModules");
            DropForeignKey("DMS.SystemPermissions", "FeatureId", "DMS.SystemFeatures");
            DropForeignKey("DMS.SystemPermissions", "AccessTypeId", "DMS.SystemAccessTypes");
            DropIndex("DMS.AdminRolePermissions", "IX_PermissionRole");
            DropIndex("DMS.SystemModules", new[] { "Code" });
            DropIndex("DMS.SystemFeatures", new[] { "Code" });
            DropIndex("DMS.SystemAccessTypes", new[] { "Code" });
            DropIndex("DMS.SystemPermissions", "IX_ModuleFeatureAccessType");
            DropIndex("DMS.SystemActions", new[] { "ObjectId" });
            DropIndex("DMS.SystemActions", "IX_ObjectCode");
            DropColumn("DMS.SystemActions", "PermissionId");
            DropTable("DMS.AdminRolePermissions");
            DropTable("DMS.SystemModules");
            DropTable("DMS.SystemFeatures");
            DropTable("DMS.SystemAccessTypes");
            DropTable("DMS.SystemPermissions");
            CreateIndex("DMS.SystemActions", new[] { "ObjectId", "Code" }, unique: true, name: "IX_ObjectCode");
        }
    }
}
