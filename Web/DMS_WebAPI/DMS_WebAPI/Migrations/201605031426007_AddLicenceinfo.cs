namespace DMS_WebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLicenceinfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetClients", "FirstStart", c => c.DateTime());
            AddColumn("dbo.AspNetClients", "LicenceType", c => c.String());
            AddColumn("dbo.AspNetClients", "NumberOfConnections", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetClients", "DurationDay", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetClients", "DurationDay");
            DropColumn("dbo.AspNetClients", "NumberOfConnections");
            DropColumn("dbo.AspNetClients", "LicenceType");
            DropColumn("dbo.AspNetClients", "FirstStart");
        }
    }
}
