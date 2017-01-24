namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SysActionPerm : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.SystemActions", "PermissionId", c => c.Int());
            CreateIndex("DMS.SystemActions", "PermissionId");
            AddForeignKey("DMS.SystemActions", "PermissionId", "DMS.SystemPermissions", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.SystemActions", "PermissionId", "DMS.SystemPermissions");
            DropIndex("DMS.SystemActions", new[] { "PermissionId" });
            DropColumn("DMS.SystemActions", "PermissionId");
        }
    }
}
