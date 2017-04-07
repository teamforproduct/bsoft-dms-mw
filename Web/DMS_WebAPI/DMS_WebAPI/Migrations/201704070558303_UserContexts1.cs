namespace DMS_WebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserContexts1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUserContexts", "UserId", c => c.String(maxLength: 128));
            AlterColumn("dbo.AspNetUserContexts", "UserName", c => c.String(maxLength: 256));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUserContexts", "UserName", c => c.String());
            AlterColumn("dbo.AspNetUserContexts", "UserId", c => c.String());
        }
    }
}
