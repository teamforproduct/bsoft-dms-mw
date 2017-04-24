namespace DMS_WebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClientRequests : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetClientRequests", "SMSCode", c => c.String(maxLength: 10));
            AddColumn("dbo.AspNetClientRequests", "HashCode", c => c.String(maxLength: 32));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetClientRequests", "HashCode");
            DropColumn("dbo.AspNetClientRequests", "SMSCode");
        }
    }
}
