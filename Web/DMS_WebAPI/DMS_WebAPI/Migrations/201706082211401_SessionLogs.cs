namespace DMS_WebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SessionLogs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SessionLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LogDate = c.DateTime(nullable: false),
                        Type = c.Int(nullable: false),
                        Event = c.String(maxLength: 40),
                        Message = c.String(maxLength: 2000),
                        UserId = c.String(maxLength: 128),
                        LastUsage = c.DateTime(),
                        Enabled = c.Boolean(nullable: false),
                        IP = c.String(),
                        Platform = c.String(),
                        Browser = c.String(),
                        Fingerprint = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.LogDate)
                .Index(t => t.UserId);
            
            AddColumn("dbo.AspNetUserContexts", "SessionId", c => c.Int());
            CreateIndex("dbo.AspNetUserContexts", "SessionId");
            AddForeignKey("dbo.AspNetUserContexts", "SessionId", "dbo.SessionLogs", "Id");
            DropColumn("dbo.AspNetUserContexts", "Fingerprint");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUserContexts", "Fingerprint", c => c.String(maxLength: 36));
            DropForeignKey("dbo.AspNetUserContexts", "SessionId", "dbo.SessionLogs");
            DropForeignKey("dbo.SessionLogs", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.SessionLogs", new[] { "UserId" });
            DropIndex("dbo.SessionLogs", new[] { "LogDate" });
            DropIndex("dbo.AspNetUserContexts", new[] { "SessionId" });
            DropColumn("dbo.AspNetUserContexts", "SessionId");
            DropTable("dbo.SessionLogs");
        }
    }
}
