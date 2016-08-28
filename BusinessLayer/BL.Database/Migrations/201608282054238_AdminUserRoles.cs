namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdminUserRoles : DbMigration
    {
        public override void Up()
        {
            DropIndex("DMS.AdminUserRoles", "IX_UserRoleStartDate");
            AddColumn("DMS.AdminUserRoles", "PositionId", c => c.Int());
            AlterColumn("DMS.EncryptionCertificates", "Name", c => c.String(maxLength: 400));
            AlterColumn("DMS.EncryptionCertificates", "Extension", c => c.String(maxLength: 400));
            AlterColumn("DMS.Documents", "AddDescription", c => c.String(maxLength: 2000));
            CreateIndex("DMS.AdminUserRoles", new[] { "UserId", "RoleId", "StartDate", "PositionId" }, unique: true, name: "IX_UserRoleStartDate");
            AddForeignKey("DMS.AdminUserRoles", "PositionId", "DMS.DictionaryPositions", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.AdminUserRoles", "PositionId", "DMS.DictionaryPositions");
            DropIndex("DMS.AdminUserRoles", "IX_UserRoleStartDate");
            AlterColumn("DMS.Documents", "AddDescription", c => c.String());
            AlterColumn("DMS.EncryptionCertificates", "Extension", c => c.String());
            AlterColumn("DMS.EncryptionCertificates", "Name", c => c.String());
            DropColumn("DMS.AdminUserRoles", "PositionId");
            CreateIndex("DMS.AdminUserRoles", new[] { "UserId", "RoleId", "StartDate" }, unique: true, name: "IX_UserRoleStartDate");
        }
    }
}
