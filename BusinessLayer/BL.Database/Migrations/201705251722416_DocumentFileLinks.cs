namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DocumentFileLinks : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "DMS.DocumentFileLinks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        FileId = c.Int(nullable: false),
                        EventId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DocumentEvents", t => t.EventId)
                .ForeignKey("DMS.DocumentFiles", t => t.FileId)
                .Index(t => t.ClientId)
                .Index(t => new { t.FileId, t.EventId }, unique: true, name: "IX_FileEvent")
                .Index(t => t.EventId, name: "IX_Event");
            
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.DocumentFileLinks", "FileId", "DMS.DocumentFiles");
            DropForeignKey("DMS.DocumentFileLinks", "EventId", "DMS.DocumentEvents");
            DropIndex("DMS.DocumentFileLinks", "IX_Event");
            DropIndex("DMS.DocumentFileLinks", "IX_FileEvent");
            DropIndex("DMS.DocumentFileLinks", new[] { "ClientId" });
            DropTable("DMS.DocumentFileLinks");
        }
    }
}
