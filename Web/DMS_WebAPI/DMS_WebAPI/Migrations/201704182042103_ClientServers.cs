namespace DMS_WebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClientServers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AspNetClientServers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        ServerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetClients", t => t.ClientId, cascadeDelete: true)
                .ForeignKey("dbo.AdminServers", t => t.ServerId, cascadeDelete: true)
                .Index(t => t.ClientId)
                .Index(t => new { t.ClientId, t.ServerId }, unique: true, name: "IX_ClientServer")
                .Index(t => t.ServerId);
            
            DropColumn("dbo.AspNetUserContexts", "UserName");
            DropColumn("dbo.AspNetUserContexts", "DatabaseId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUserContexts", "DatabaseId", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUserContexts", "UserName", c => c.String(maxLength: 256));
            DropForeignKey("dbo.AspNetClientServers", "ServerId", "dbo.AdminServers");
            DropForeignKey("dbo.AspNetClientServers", "ClientId", "dbo.AspNetClients");
            DropIndex("dbo.AspNetClientServers", new[] { "ServerId" });
            DropIndex("dbo.AspNetClientServers", "IX_ClientServer");
            DropIndex("dbo.AspNetClientServers", new[] { "ClientId" });
            DropTable("dbo.AspNetClientServers");
        }
    }
}
