namespace DMS_WebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserInvitations : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.AspNetUsers", name: "FirstName", newName: "FullName");
            CreateTable(
                "dbo.AspNetClientInvitations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        UserEmail = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetClients", t => t.ClientId, cascadeDelete: true)
                .Index(t => t.ClientId);
            
            DropColumn("dbo.AspNetUsers", "LastName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "LastName", c => c.String(maxLength: 2000));
            DropForeignKey("dbo.AspNetClientInvitations", "ClientId", "dbo.AspNetClients");
            DropIndex("dbo.AspNetClientInvitations", new[] { "ClientId" });
            DropTable("dbo.AspNetClientInvitations");
            RenameColumn(table: "dbo.AspNetUsers", name: "FullName", newName: "FirstName");
        }
    }
}
