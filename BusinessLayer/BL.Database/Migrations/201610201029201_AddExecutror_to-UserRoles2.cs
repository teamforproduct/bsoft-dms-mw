namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddExecutror_toUserRoles2 : DbMigration
    {
        public override void Up()
        {
            DropIndex("DMS.AdminUserRoles", "IX_UserRoleStartDate");
            CreateIndex("DMS.AdminUserRoles", new[] { "UserId", "RoleId", "StartDate", "PositionExecutorId" }, unique: true, name: "IX_UserRoleStartDate");
            CreateIndex("DMS.AdminUserRoles", "PositionId");
            AddForeignKey("DMS.AdminUserRoles", "PositionExecutorId", "DMS.DictionaryPositionExecutors", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.AdminUserRoles", "PositionExecutorId", "DMS.DictionaryPositionExecutors");
            DropIndex("DMS.AdminUserRoles", new[] { "PositionId" });
            DropIndex("DMS.AdminUserRoles", "IX_UserRoleStartDate");
            CreateIndex("DMS.AdminUserRoles", new[] { "UserId", "RoleId", "StartDate", "PositionId" }, unique: true, name: "IX_UserRoleStartDate");
        }
    }
}
