namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sendList2 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.TemplateDocumentSendLists", new[] { "TemplateDocuments_Id" });
            DropColumn("dbo.TemplateDocumentSendLists", "DocumentId");
            RenameColumn(table: "dbo.TemplateDocumentSendLists", name: "TemplateDocuments_Id", newName: "DocumentId");
            AlterColumn("dbo.DocumentSendLists", "OrderNumber", c => c.Int());
            AlterColumn("dbo.DocumentSendLists", "DueDay", c => c.Int());
            AlterColumn("dbo.TemplateDocumentSendLists", "OrderNumber", c => c.Int());
            AlterColumn("dbo.TemplateDocumentSendLists", "DueDay", c => c.Int());
            AlterColumn("dbo.TemplateDocumentSendLists", "DocumentId", c => c.Int(nullable: false));
            CreateIndex("dbo.TemplateDocumentSendLists", "DocumentId");
            CreateIndex("dbo.TemplateDocumentSendLists", "SendTypeId");
            CreateIndex("dbo.TemplateDocumentSendLists", "TargetPositionId");
            CreateIndex("dbo.TemplateDocumentSendLists", "AccessLevelId");
            AddForeignKey("dbo.TemplateDocumentSendLists", "AccessLevelId", "dbo.AdminAccessLevels", "Id");
            AddForeignKey("dbo.TemplateDocumentSendLists", "SendTypeId", "dbo.DictionarySendTypes", "Id");
            AddForeignKey("dbo.TemplateDocumentSendLists", "TargetPositionId", "dbo.DictionaryPositions", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TemplateDocumentSendLists", "TargetPositionId", "dbo.DictionaryPositions");
            DropForeignKey("dbo.TemplateDocumentSendLists", "SendTypeId", "dbo.DictionarySendTypes");
            DropForeignKey("dbo.TemplateDocumentSendLists", "AccessLevelId", "dbo.AdminAccessLevels");
            DropIndex("dbo.TemplateDocumentSendLists", new[] { "AccessLevelId" });
            DropIndex("dbo.TemplateDocumentSendLists", new[] { "TargetPositionId" });
            DropIndex("dbo.TemplateDocumentSendLists", new[] { "SendTypeId" });
            DropIndex("dbo.TemplateDocumentSendLists", new[] { "DocumentId" });
            AlterColumn("dbo.TemplateDocumentSendLists", "DocumentId", c => c.Int());
            AlterColumn("dbo.TemplateDocumentSendLists", "DueDay", c => c.Int(nullable: false));
            AlterColumn("dbo.TemplateDocumentSendLists", "OrderNumber", c => c.Int(nullable: false));
            AlterColumn("dbo.DocumentSendLists", "DueDay", c => c.Int(nullable: false));
            AlterColumn("dbo.DocumentSendLists", "OrderNumber", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.TemplateDocumentSendLists", name: "DocumentId", newName: "TemplateDocuments_Id");
            AddColumn("dbo.TemplateDocumentSendLists", "DocumentId", c => c.Int(nullable: false));
            CreateIndex("dbo.TemplateDocumentSendLists", "TemplateDocuments_Id");
        }
    }
}
