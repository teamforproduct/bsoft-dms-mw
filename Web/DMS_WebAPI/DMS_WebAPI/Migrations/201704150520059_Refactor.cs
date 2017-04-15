namespace DMS_WebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Refactor : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.AspNetClientServers", newName: "AspNetUserClientServers");
            DropIndex("dbo.AspNetUserClientServers", new[] { "ServerId" });
            DropIndex("dbo.AspNetUserServers", new[] { "UserId" });
            DropIndex("dbo.AspNetUserServers", new[] { "ServerId" });
            DropIndex("dbo.AspNetUserServers", new[] { "ClientId" });
            DropIndex("dbo.AspNetUserClients", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClients", new[] { "ClientId" });
            DropColumn("dbo.AspNetUserClientServers", "ClientId");
            RenameColumn(table: "dbo.AspNetUserClientServers", name: "ServerId", newName: "ClientId");
            AddColumn("dbo.AspNetUserClientServers", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.AspNetUserClientServers", "UserId");
            DropTable("dbo.AspNetUserServers");
            DropTable("dbo.AspNetUserClients");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.AspNetUserClients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        ClientId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserServers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        ServerId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropIndex("dbo.AspNetUserClientServers", new[] { "UserId" });
            DropColumn("dbo.AspNetUserClientServers", "UserId");
            RenameColumn(table: "dbo.AspNetUserClientServers", name: "ClientId", newName: "ServerId");
            AddColumn("dbo.AspNetUserClientServers", "ClientId", c => c.Int(nullable: false));
            CreateIndex("dbo.AspNetUserClients", "ClientId");
            CreateIndex("dbo.AspNetUserClients", "UserId");
            CreateIndex("dbo.AspNetUserServers", "ClientId");
            CreateIndex("dbo.AspNetUserServers", "ServerId");
            CreateIndex("dbo.AspNetUserServers", "UserId");
            CreateIndex("dbo.AspNetUserClientServers", "ServerId");
            RenameTable(name: "dbo.AspNetUserClientServers", newName: "AspNetClientServers");
        }
    }
}
