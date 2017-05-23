namespace DMS_WebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserClients : DbMigration
    {
        public override void Up()
        {
            
            DropForeignKey("dbo.AspNetUserClientServers", "ServerId", "dbo.AdminServers");
            RenameTable(name: "dbo.AspNetUserClientServers", newName: "AspNetUserClients");
            DropIndex("dbo.AspNetUserClients", "IX_UserClientServer");
            DropIndex("dbo.AspNetUserClients", new[] { "ServerId" });
            CreateIndex("dbo.AspNetUserClients", new[] { "UserId", "ClientId" }, unique: true, name: "IX_UserClientServer");
            DropColumn("dbo.AspNetUserClients", "ServerId");
            
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUserClients", "ServerId", c => c.Int(nullable: false));
            DropIndex("dbo.AspNetUserClients", "IX_UserClientServer");
            CreateIndex("dbo.AspNetUserClients", "ServerId");
            CreateIndex("dbo.AspNetUserClients", new[] { "UserId", "ClientId", "ServerId" }, unique: true, name: "IX_UserClientServer");
            AddForeignKey("dbo.AspNetUserClientServers", "ServerId", "dbo.AdminServers", "Id", cascadeDelete: true);
            RenameTable(name: "dbo.AspNetUserClients", newName: "AspNetUserClientServers");
        }
    }
}
