namespace DMS_WebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserClientServer_Unique : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.AspNetUserClientServers", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClientServers", new[] { "ServerId" });
            DropIndex("dbo.AspNetUserClientServers", new[] { "ClientId" });
            CreateIndex("dbo.AspNetUserClientServers", new[] { "UserId", "ClientId", "ServerId" }, unique: true, name: "IX_UserClientServer");
            CreateIndex("dbo.AspNetUserClientServers", "UserId");
            CreateIndex("dbo.AspNetUserClientServers", "ClientId");
            CreateIndex("dbo.AspNetUserClientServers", "ServerId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.AspNetUserClientServers", new[] { "ServerId" });
            DropIndex("dbo.AspNetUserClientServers", new[] { "ClientId" });
            DropIndex("dbo.AspNetUserClientServers", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClientServers", "IX_UserClientServer");
            CreateIndex("dbo.AspNetUserClientServers", "ClientId");
            CreateIndex("dbo.AspNetUserClientServers", "ServerId");
            CreateIndex("dbo.AspNetUserClientServers", "UserId");
        }
    }
}
