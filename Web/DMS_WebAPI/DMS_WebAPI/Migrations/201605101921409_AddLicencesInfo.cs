namespace DMS_WebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLicencesInfo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AspNetLicences",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsTrial = c.Boolean(nullable: false),
                        NamedNumberOfConnections = c.Int(),
                        ConcurenteNumberOfConnections = c.Int(),
                        DurationDay = c.Int(),
                        Functionals = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.AspNetClients", "IsTrial", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetClients", "NamedNumberOfConnections", c => c.Int());
            AddColumn("dbo.AspNetClients", "ConcurenteNumberOfConnections", c => c.Int());
            AddColumn("dbo.AspNetClients", "Functionals", c => c.String());
            AlterColumn("dbo.AspNetClients", "DurationDay", c => c.Int());
            DropColumn("dbo.AspNetClients", "LicenceType");
            DropColumn("dbo.AspNetClients", "NumberOfConnections");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetClients", "NumberOfConnections", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetClients", "LicenceType", c => c.String());
            AlterColumn("dbo.AspNetClients", "DurationDay", c => c.Int(nullable: false));
            DropColumn("dbo.AspNetClients", "Functionals");
            DropColumn("dbo.AspNetClients", "ConcurenteNumberOfConnections");
            DropColumn("dbo.AspNetClients", "NamedNumberOfConnections");
            DropColumn("dbo.AspNetClients", "IsTrial");
            DropTable("dbo.AspNetLicences");
        }
    }
}
