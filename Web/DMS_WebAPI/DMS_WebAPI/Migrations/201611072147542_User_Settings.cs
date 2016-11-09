namespace DMS_WebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class User_Settings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "IsChangePasswordRequired", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "IsEmailConfirmRequired", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "IsEmailConfirmRequired");
            DropColumn("dbo.AspNetUsers", "IsChangePasswordRequired");
        }
    }
}
