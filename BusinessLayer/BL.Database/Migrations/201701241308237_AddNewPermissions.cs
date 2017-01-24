namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewPermissions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "DMS.SystemPermissions",
                c => new
                    {
                        Id = c.Int(nullable: false),
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
                        Id = c.Int(nullable: false),
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
                        Id = c.Int(nullable: false),
                        ModuleId = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 2000),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.SystemModules", t => t.ModuleId)
                .Index(t => new { t.ModuleId, t.Code }, unique: true, name: "IX_ModuleCode");
            
            CreateTable(
                "DMS.SystemModules",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 2000),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true);
            
            CreateIndex("DMS.SystemActions", "PermissionId");
            AddForeignKey("DMS.AdminRolePermissions", "PermissionId", "DMS.SystemPermissions", "Id");
            AddForeignKey("DMS.SystemActions", "PermissionId", "DMS.SystemPermissions", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.SystemActions", "PermissionId", "DMS.SystemPermissions");
            DropForeignKey("DMS.AdminRolePermissions", "PermissionId", "DMS.SystemPermissions");
            DropForeignKey("DMS.SystemPermissions", "ModuleId", "DMS.SystemModules");
            DropForeignKey("DMS.SystemPermissions", "FeatureId", "DMS.SystemFeatures");
            DropForeignKey("DMS.SystemFeatures", "ModuleId", "DMS.SystemModules");
            DropForeignKey("DMS.SystemPermissions", "AccessTypeId", "DMS.SystemAccessTypes");
            DropIndex("DMS.SystemModules", new[] { "Code" });
            DropIndex("DMS.SystemFeatures", "IX_ModuleCode");
            DropIndex("DMS.SystemAccessTypes", new[] { "Code" });
            DropIndex("DMS.SystemPermissions", "IX_ModuleFeatureAccessType");
            DropIndex("DMS.SystemActions", new[] { "PermissionId" });
            DropTable("DMS.SystemModules");
            DropTable("DMS.SystemFeatures");
            DropTable("DMS.SystemAccessTypes");
            DropTable("DMS.SystemPermissions");
        }
    }
}
