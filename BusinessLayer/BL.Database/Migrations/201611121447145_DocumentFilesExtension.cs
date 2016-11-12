namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DocumentFilesExtension : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("DMS.Files", "EntityId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.Files", "EntityId", "DMS.Documents");
            DropForeignKey("DMS.Files", "ExecutorPositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.Files", "ExecutorPositionExeAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.Files", "TypeId", "DMS.DictionaryFileTypes");
            DropIndex("DMS.DocumentFiles", "IX_DocumentNameExtensionVersion");
            DropIndex("DMS.DocumentFiles", "IX_DocumentOrderNumberVersion");
            DropIndex("DMS.Files", "IX_EntityObjectNameExtensionVersion");
            DropIndex("DMS.Files", "IX_EntityObjectOrderNumberVersion");
            DropIndex("DMS.Files", new[] { "TypeId" });
            DropIndex("DMS.Files", new[] { "ExecutorPositionId" });
            DropIndex("DMS.Files", new[] { "ExecutorPositionExeAgentId" });
            DropIndex("DMS.Files", new[] { "LastChangeDate" });
            AddColumn("DMS.DocumentFiles", "EntityId", c => c.Int(nullable: false));
            AddColumn("DMS.DocumentFiles", "ObjectId", c => c.Int(nullable: false));
            AlterColumn("DMS.DocumentFiles", "Extension", c => c.String(maxLength: 20));
            CreateIndex("DMS.DocumentFiles", new[] { "EntityId", "ObjectId", "Name", "Extension", "Version" }, unique: true, name: "IX_EntityObjectNameExtensionVersion");
            CreateIndex("DMS.DocumentFiles", new[] { "EntityId", "ObjectId", "OrderNumber", "Version" }, unique: true, name: "IX_EntityObjectOrderNumberVersion");
            CreateIndex("DMS.DocumentFiles", "DocumentId");
            AddForeignKey("DMS.DocumentFiles", "ObjectId", "DMS.SystemObjects", "Id");
            DropTable("DMS.Files");
        }
        
        public override void Down()
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
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("DMS.DocumentFiles", "ObjectId", "DMS.SystemObjects");
            DropIndex("DMS.DocumentFiles", new[] { "DocumentId" });
            DropIndex("DMS.DocumentFiles", "IX_EntityObjectOrderNumberVersion");
            DropIndex("DMS.DocumentFiles", "IX_EntityObjectNameExtensionVersion");
            AlterColumn("DMS.DocumentFiles", "Extension", c => c.String(maxLength: 200));
            DropColumn("DMS.DocumentFiles", "ObjectId");
            DropColumn("DMS.DocumentFiles", "EntityId");
            CreateIndex("DMS.Files", "LastChangeDate");
            CreateIndex("DMS.Files", "ExecutorPositionExeAgentId");
            CreateIndex("DMS.Files", "ExecutorPositionId");
            CreateIndex("DMS.Files", "TypeId");
            CreateIndex("DMS.Files", new[] { "EntityId", "ObjectId", "OrderNumber", "Version" }, unique: true, name: "IX_EntityObjectOrderNumberVersion");
            CreateIndex("DMS.Files", new[] { "EntityId", "ObjectId", "Name", "Extension", "Version" }, unique: true, name: "IX_EntityObjectNameExtensionVersion");
            CreateIndex("DMS.DocumentFiles", new[] { "DocumentId", "OrderNumber", "Version" }, unique: true, name: "IX_DocumentOrderNumberVersion");
            CreateIndex("DMS.DocumentFiles", new[] { "DocumentId", "Name", "Extension", "Version" }, unique: true, name: "IX_DocumentNameExtensionVersion");
            AddForeignKey("DMS.Files", "TypeId", "DMS.DictionaryFileTypes", "Id");
            AddForeignKey("DMS.Files", "ExecutorPositionExeAgentId", "DMS.DictionaryAgents", "Id");
            AddForeignKey("DMS.Files", "ExecutorPositionId", "DMS.DictionaryPositions", "Id");
            AddForeignKey("DMS.Files", "EntityId", "DMS.Documents", "Id");
            AddForeignKey("DMS.Files", "EntityId", "DMS.DictionaryAgents", "Id");
        }
    }
}
