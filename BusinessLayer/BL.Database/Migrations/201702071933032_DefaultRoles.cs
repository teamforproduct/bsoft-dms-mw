namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DefaultRoles : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("DMS.AdminRoleActions", "ActionId", "DMS.SystemActions");
            DropForeignKey("DMS.AdminRoleActions", "RoleId", "DMS.AdminRoles");
            DropForeignKey("DMS.AdminRoles", "RoleTypeId", "DMS.AdminRoleTypes");
            DropIndex("DMS.AdminRoleActions", "IX_ActionRoleRecord");
            DropIndex("DMS.AdminRoleActions", new[] { "RoleId" });
            DropPrimaryKey("DMS.AdminRoleTypes");
            AlterColumn("DMS.AdminRoleTypes", "Id", c => c.Int(nullable: false));
            AddPrimaryKey("DMS.AdminRoleTypes", "Id");
            AddForeignKey("DMS.AdminRoles", "RoleTypeId", "DMS.AdminRoleTypes", "Id");
            DropTable("DMS.AdminRoleActions");
        }
        
        public override void Down()
        {
            CreateTable(
                "DMS.AdminRoleActions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoleId = c.Int(nullable: false),
                        ActionId = c.Int(nullable: false),
                        RecordId = c.Int(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("DMS.AdminRoles", "RoleTypeId", "DMS.AdminRoleTypes");
            DropPrimaryKey("DMS.AdminRoleTypes");
            AlterColumn("DMS.AdminRoleTypes", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("DMS.AdminRoleTypes", "Id");
            CreateIndex("DMS.AdminRoleActions", "RoleId");
            CreateIndex("DMS.AdminRoleActions", new[] { "ActionId", "RoleId", "RecordId" }, unique: true, name: "IX_ActionRoleRecord");
            AddForeignKey("DMS.AdminRoles", "RoleTypeId", "DMS.AdminRoleTypes", "Id");
            AddForeignKey("DMS.AdminRoleActions", "RoleId", "DMS.AdminRoles", "Id");
            AddForeignKey("DMS.AdminRoleActions", "ActionId", "DMS.SystemActions", "Id");
        }
    }
}
