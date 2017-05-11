namespace DMS_WebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateDate_ClientRequests : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetClientRequests", "CreateDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetClientRequests", "CreateDate");
        }
    }
}
