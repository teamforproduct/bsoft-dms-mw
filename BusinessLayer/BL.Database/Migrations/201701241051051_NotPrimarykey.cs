namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NotPrimarykey : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("DMS.SystemActions", "PermissionId", "DMS.SystemPermissions");
            DropForeignKey("DMS.AdminRolePermissions", "PermissionId", "DMS.SystemPermissions");
            DropForeignKey("DMS.SystemPermissions", "FeatureId", "DMS.SystemFeatures");
            DropForeignKey("DMS.SystemFeatures", "ModuleId", "DMS.SystemModules");
            DropForeignKey("DMS.SystemPermissions", "ModuleId", "DMS.SystemModules");
            DropIndex("DMS.SystemActions", "IX_ObjectCode");
            DropIndex("DMS.SystemActions", new[] { "ObjectId" });
            DropIndex("DMS.SystemFeatures", new[] { "ModuleId" });
            DropPrimaryKey("DMS.SystemPermissions");
            DropPrimaryKey("DMS.SystemFeatures");
            DropPrimaryKey("DMS.SystemModules");
            AlterColumn("DMS.SystemPermissions", "Id", c => c.Int(nullable: false));
            AlterColumn("DMS.SystemFeatures", "Id", c => c.Int(nullable: false));
            AlterColumn("DMS.SystemModules", "Id", c => c.Int(nullable: false));
            AddPrimaryKey("DMS.SystemPermissions", "Id");
            AddPrimaryKey("DMS.SystemFeatures", "Id");
            AddPrimaryKey("DMS.SystemModules", "Id");
            CreateIndex("DMS.SystemActions", new[] { "ObjectId", "Code" }, unique: true, name: "IX_ObjectCode");
            CreateIndex("DMS.SystemFeatures", new[] { "ModuleId", "Code" }, unique: true, name: "IX_ModuleCode");
            CreateIndex("DMS.SystemModules", "Code", unique: true);
            AddForeignKey("DMS.AdminRolePermissions", "PermissionId", "DMS.SystemPermissions", "Id");
            AddForeignKey("DMS.SystemPermissions", "FeatureId", "DMS.SystemFeatures", "Id");
            AddForeignKey("DMS.SystemFeatures", "ModuleId", "DMS.SystemModules", "Id");
            AddForeignKey("DMS.SystemPermissions", "ModuleId", "DMS.SystemModules", "Id");
            DropColumn("DMS.SystemActions", "PermissionId");
        }
        
        public override void Down()
        {
            AddColumn("DMS.SystemActions", "PermissionId", c => c.Int(nullable: false));
            DropForeignKey("DMS.SystemPermissions", "ModuleId", "DMS.SystemModules");
            DropForeignKey("DMS.SystemFeatures", "ModuleId", "DMS.SystemModules");
            DropForeignKey("DMS.SystemPermissions", "FeatureId", "DMS.SystemFeatures");
            DropForeignKey("DMS.AdminRolePermissions", "PermissionId", "DMS.SystemPermissions");
            DropIndex("DMS.SystemModules", new[] { "Code" });
            DropIndex("DMS.SystemFeatures", "IX_ModuleCode");
            DropIndex("DMS.SystemActions", "IX_ObjectCode");
            DropPrimaryKey("DMS.SystemModules");
            DropPrimaryKey("DMS.SystemFeatures");
            DropPrimaryKey("DMS.SystemPermissions");
            AlterColumn("DMS.SystemModules", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("DMS.SystemFeatures", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("DMS.SystemPermissions", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("DMS.SystemModules", "Id");
            AddPrimaryKey("DMS.SystemFeatures", "Id");
            AddPrimaryKey("DMS.SystemPermissions", "Id");
            CreateIndex("DMS.SystemFeatures", "ModuleId");
            CreateIndex("DMS.SystemActions", "ObjectId");
            CreateIndex("DMS.SystemActions", new[] { "PermissionId", "Code" }, unique: true, name: "IX_ObjectCode");
            AddForeignKey("DMS.SystemPermissions", "ModuleId", "DMS.SystemModules", "Id");
            AddForeignKey("DMS.SystemFeatures", "ModuleId", "DMS.SystemModules", "Id");
            AddForeignKey("DMS.SystemPermissions", "FeatureId", "DMS.SystemFeatures", "Id");
            AddForeignKey("DMS.AdminRolePermissions", "PermissionId", "DMS.SystemPermissions", "Id");
            AddForeignKey("DMS.SystemActions", "PermissionId", "DMS.SystemPermissions", "Id");
        }
    }
}
