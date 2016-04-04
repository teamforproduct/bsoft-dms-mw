namespace DMS_WebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Clients : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AspNetClients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserServers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        ServerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AdminServers", t => t.ServerId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.ServerId);
            
            AddColumn("dbo.AdminServers", "ClientId", c => c.Int());
            CreateIndex("dbo.AdminServers", "ClientId");
            AddForeignKey("dbo.AdminServers", "ClientId", "dbo.AspNetClients", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserServers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserServers", "ServerId", "dbo.AdminServers");
            DropForeignKey("dbo.AdminServers", "ClientId", "dbo.AspNetClients");
            DropIndex("dbo.AspNetUserServers", new[] { "ServerId" });
            DropIndex("dbo.AspNetUserServers", new[] { "UserId" });
            DropIndex("dbo.AdminServers", new[] { "ClientId" });
            DropColumn("dbo.AdminServers", "ClientId");
            DropTable("dbo.AspNetUserServers");
            DropTable("dbo.AspNetClients");
        }
    }
}
