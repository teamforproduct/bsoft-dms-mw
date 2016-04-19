namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdminIndex : DbMigration
    {
        public override void Up()
        {
            DropIndex("DMS.AdminLanguageValues", new[] { "LanguageId" });
            DropIndex("DMS.AdminPositionRoles", new[] { "RoleId" });
            DropIndex("DMS.AdminPositionRoles", new[] { "PositionId" });
            DropIndex("DMS.AdminRoleActions", new[] { "RoleId" });
            DropIndex("DMS.AdminRoleActions", new[] { "ActionId" });
            DropIndex("DMS.AdminUserRoles", new[] { "UserId" });
            DropIndex("DMS.AdminUserRoles", new[] { "RoleId" });
            DropIndex("DMS.AdminSubordinations", new[] { "SourcePositionId" });
            DropIndex("DMS.AdminSubordinations", new[] { "TargetPositionId" });
            DropIndex("DMS.AdminSubordinations", new[] { "SubordinationTypeId" });
            CreateIndex("DMS.AdminAccessLevels", "Name", unique: true);
            CreateIndex("DMS.AdminLanguages", "Code", unique: true);
            CreateIndex("DMS.AdminLanguages", "Name", unique: true);
            CreateIndex("DMS.AdminLanguageValues", new[] { "Label", "LanguageId" }, unique: true, name: "IX_Label");
            CreateIndex("DMS.AdminLanguageValues", "LanguageId");
            CreateIndex("DMS.AdminPositionRoles", new[] { "PositionId", "RoleId" }, unique: true, name: "IX_PositionRole");
            CreateIndex("DMS.AdminPositionRoles", "RoleId");
            CreateIndex("DMS.AdminRoles", "Name", unique: true);
            CreateIndex("DMS.AdminRoleActions", new[] { "ActionId", "RoleId", "RecordId" }, unique: true, name: "IX_ActionRoleRecord");
            CreateIndex("DMS.AdminRoleActions", "RoleId");
            CreateIndex("DMS.AdminUserRoles", new[] { "UserId", "RoleId", "StartDate" }, unique: true, name: "IX_UserRoleStartDate");
            CreateIndex("DMS.AdminUserRoles", "RoleId");
            CreateIndex("DMS.AdminSubordinations", new[] { "SourcePositionId", "TargetPositionId", "SubordinationTypeId" }, unique: true, name: "IX_SourceTargetType");
            CreateIndex("DMS.AdminSubordinations", "TargetPositionId");
            CreateIndex("DMS.AdminSubordinations", "SubordinationTypeId");
            DropColumn("DMS.AdminRoles", "PositionId");
            DropColumn("DMS.AdminRoles", "AccessLevelId");
        }
        
        public override void Down()
        {
            AddColumn("DMS.AdminRoles", "AccessLevelId", c => c.Int(nullable: false));
            AddColumn("DMS.AdminRoles", "PositionId", c => c.Int(nullable: false));
            DropIndex("DMS.AdminSubordinations", new[] { "SubordinationTypeId" });
            DropIndex("DMS.AdminSubordinations", new[] { "TargetPositionId" });
            DropIndex("DMS.AdminSubordinations", "IX_SourceTargetType");
            DropIndex("DMS.AdminUserRoles", new[] { "RoleId" });
            DropIndex("DMS.AdminUserRoles", "IX_UserRoleStartDate");
            DropIndex("DMS.AdminRoleActions", new[] { "RoleId" });
            DropIndex("DMS.AdminRoleActions", "IX_ActionRoleRecord");
            DropIndex("DMS.AdminRoles", new[] { "Name" });
            DropIndex("DMS.AdminPositionRoles", new[] { "RoleId" });
            DropIndex("DMS.AdminPositionRoles", "IX_PositionRole");
            DropIndex("DMS.AdminLanguageValues", new[] { "LanguageId" });
            DropIndex("DMS.AdminLanguageValues", "IX_Label");
            DropIndex("DMS.AdminLanguages", new[] { "Name" });
            DropIndex("DMS.AdminLanguages", new[] { "Code" });
            DropIndex("DMS.AdminAccessLevels", new[] { "Name" });
            CreateIndex("DMS.AdminSubordinations", "SubordinationTypeId");
            CreateIndex("DMS.AdminSubordinations", "TargetPositionId");
            CreateIndex("DMS.AdminSubordinations", "SourcePositionId");
            CreateIndex("DMS.AdminUserRoles", "RoleId");
            CreateIndex("DMS.AdminUserRoles", "UserId");
            CreateIndex("DMS.AdminRoleActions", "ActionId");
            CreateIndex("DMS.AdminRoleActions", "RoleId");
            CreateIndex("DMS.AdminPositionRoles", "PositionId");
            CreateIndex("DMS.AdminPositionRoles", "RoleId");
            CreateIndex("DMS.AdminLanguageValues", "LanguageId");
        }
    }
}
