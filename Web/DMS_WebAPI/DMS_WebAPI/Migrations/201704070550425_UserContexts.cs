namespace DMS_WebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserContexts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUserContexts", "UserName", c => c.String());
            AddColumn("dbo.AspNetUserContexts", "LastChangeDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.AspNetUserContexts", "Token", c => c.String(maxLength: 550));
            AlterColumn("dbo.AspNetUserContexts", "CurrentPositionsIdList", c => c.String(maxLength: 400));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUserContexts", "CurrentPositionsIdList", c => c.String(maxLength: 2000));
            AlterColumn("dbo.AspNetUserContexts", "Token", c => c.String(maxLength: 2000));
            DropColumn("dbo.AspNetUserContexts", "LastChangeDate");
            DropColumn("dbo.AspNetUserContexts", "UserName");
        }
    }
}
