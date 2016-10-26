namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDescriptionToRoles : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.AdminRoles", "Description", c => c.String(maxLength: 2000));
            AddColumn("DMS.SystemActions", "IsVisibleInMenu", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("DMS.SystemActions", "IsVisibleInMenu");
            DropColumn("DMS.AdminRoles", "Description");
        }
    }
}
