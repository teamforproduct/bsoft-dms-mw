namespace DMS_WebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserContextsKey : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.AspNetUserContexts", name: "SessionId", newName: "SignInId");
            RenameIndex(table: "dbo.AspNetUserContexts", name: "IX_SessionId", newName: "IX_SignInId");
            AddColumn("dbo.AspNetUserContexts", "Key", c => c.String(maxLength: 40));
            CreateIndex("dbo.AspNetUserContexts", "Key");
            DropColumn("dbo.AspNetUserContexts", "Token");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUserContexts", "Token", c => c.String(maxLength: 550));
            DropIndex("dbo.AspNetUserContexts", new[] { "Key" });
            DropColumn("dbo.AspNetUserContexts", "Key");
            RenameIndex(table: "dbo.AspNetUserContexts", name: "IX_SignInId", newName: "IX_SessionId");
            RenameColumn(table: "dbo.AspNetUserContexts", name: "SignInId", newName: "SessionId");
        }
    }
}
