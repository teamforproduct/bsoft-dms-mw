namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Files : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "DMS.Files",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EntityId = c.Int(nullable: false),
                        ObjectId = c.Int(nullable: false),
                        Name = c.String(maxLength: 200),
                        OrderNumber = c.Int(nullable: false),
                        Version = c.Int(nullable: false),
                        Extension = c.String(maxLength: 200),
                        FileType = c.String(maxLength: 2000),
                        FileSize = c.Long(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Content = c.String(maxLength: 2000),
                        TypeId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        IsWorkedOut = c.Boolean(),
                        Hash = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 2000),
                        IsMainVersion = c.Boolean(nullable: false),
                        ExecutorPositionId = c.Int(nullable: false),
                        ExecutorPositionExeAgentId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.EntityId)
                .ForeignKey("DMS.Documents", t => t.EntityId)
                .ForeignKey("DMS.DictionaryPositions", t => t.ExecutorPositionId)
                .ForeignKey("DMS.DictionaryAgents", t => t.ExecutorPositionExeAgentId)
                .ForeignKey("DMS.DictionaryFileTypes", t => t.TypeId)
                .Index(t => new { t.EntityId, t.ObjectId, t.Name, t.Extension, t.Version }, unique: true, name: "IX_EntityObjectNameExtensionVersion")
                .Index(t => new { t.EntityId, t.ObjectId, t.OrderNumber, t.Version }, unique: true, name: "IX_EntityObjectOrderNumberVersion")
                .Index(t => t.TypeId)
                .Index(t => t.ExecutorPositionId)
                .Index(t => t.ExecutorPositionExeAgentId)
                .Index(t => t.LastChangeDate);
            
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.Files", "TypeId", "DMS.DictionaryFileTypes");
            DropForeignKey("DMS.Files", "ExecutorPositionExeAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.Files", "ExecutorPositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.Files", "EntityId", "DMS.Documents");
            DropForeignKey("DMS.Files", "EntityId", "DMS.DictionaryAgents");
            DropIndex("DMS.Files", new[] { "LastChangeDate" });
            DropIndex("DMS.Files", new[] { "ExecutorPositionExeAgentId" });
            DropIndex("DMS.Files", new[] { "ExecutorPositionId" });
            DropIndex("DMS.Files", new[] { "TypeId" });
            DropIndex("DMS.Files", "IX_EntityObjectOrderNumberVersion");
            DropIndex("DMS.Files", "IX_EntityObjectNameExtensionVersion");
            DropTable("DMS.Files");
        }
    }
}
