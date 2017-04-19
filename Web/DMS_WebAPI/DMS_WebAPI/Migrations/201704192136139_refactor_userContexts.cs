namespace DMS_WebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class refactor_userContexts : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUserContexts", "IsChangePasswordRequired");
            DropColumn("dbo.AspNetUserContexts", "LoginLogId");
            DropColumn("dbo.AspNetUserContexts", "LoginLogInfo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUserContexts", "LoginLogInfo", c => c.String(maxLength: 2000));
            AddColumn("dbo.AspNetUserContexts", "LoginLogId", c => c.Int());
            AddColumn("dbo.AspNetUserContexts", "IsChangePasswordRequired", c => c.Boolean(nullable: false));
        }
    }
}
