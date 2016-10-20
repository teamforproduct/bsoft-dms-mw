namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddExecutror_toUserRoles : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.AdminUserRoles", "PositionExecutorId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("DMS.AdminUserRoles", "PositionExecutorId");
        }
    }
}
