namespace DMS_WebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnLicenceKey : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetClients", "LicenceKey", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetClients", "LicenceKey");
        }
    }
}
