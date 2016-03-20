namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sendList2 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.TemplateDocumentSendListsSet", new[] { "TemplateDocuments_Id" });
            DropColumn("dbo.TemplateDocumentSendListsSet", "DocumentId");
            RenameColumn(table: "dbo.TemplateDocumentSendListsSet", name: "TemplateDocuments_Id", newName: "DocumentId");
            AlterColumn("dbo.DocumentSendLists", "OrderNumber", c => c.Int());
            AlterColumn("dbo.DocumentSendLists", "DueDay", c => c.Int());
            AlterColumn("dbo.TemplateDocumentSendListsSet", "OrderNumber", c => c.Int());
            AlterColumn("dbo.TemplateDocumentSendListsSet", "DueDay", c => c.Int());
            AlterColumn("dbo.TemplateDocumentSendListsSet", "DocumentId", c => c.Int(nullable: false));
            CreateIndex("dbo.TemplateDocumentSendListsSet", "DocumentId");
            CreateIndex("dbo.TemplateDocumentSendListsSet", "SendTypeId");
            CreateIndex("dbo.TemplateDocumentSendListsSet", "TargetPositionId");
            CreateIndex("dbo.TemplateDocumentSendListsSet", "AccessLevelId");
            AddForeignKey("dbo.TemplateDocumentSendListsSet", "AccessLevelId", "dbo.AdminAccessLevels", "Id");
            AddForeignKey("dbo.TemplateDocumentSendListsSet", "SendTypeId", "dbo.DictionarySendTypes", "Id");
            AddForeignKey("dbo.TemplateDocumentSendListsSet", "TargetPositionId", "dbo.DictionaryPositions", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TemplateDocumentSendListsSet", "TargetPositionId", "dbo.DictionaryPositions");
            DropForeignKey("dbo.TemplateDocumentSendListsSet", "SendTypeId", "dbo.DictionarySendTypes");
            DropForeignKey("dbo.TemplateDocumentSendListsSet", "AccessLevelId", "dbo.AdminAccessLevels");
            DropIndex("dbo.TemplateDocumentSendListsSet", new[] { "AccessLevelId" });
            DropIndex("dbo.TemplateDocumentSendListsSet", new[] { "TargetPositionId" });
            DropIndex("dbo.TemplateDocumentSendListsSet", new[] { "SendTypeId" });
            DropIndex("dbo.TemplateDocumentSendListsSet", new[] { "DocumentId" });
            AlterColumn("dbo.TemplateDocumentSendListsSet", "DocumentId", c => c.Int());
            AlterColumn("dbo.TemplateDocumentSendListsSet", "DueDay", c => c.Int(nullable: false));
            AlterColumn("dbo.TemplateDocumentSendListsSet", "OrderNumber", c => c.Int(nullable: false));
            AlterColumn("dbo.DocumentSendLists", "DueDay", c => c.Int(nullable: false));
            AlterColumn("dbo.DocumentSendLists", "OrderNumber", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.TemplateDocumentSendListsSet", name: "DocumentId", newName: "TemplateDocuments_Id");
            AddColumn("dbo.TemplateDocumentSendListsSet", "DocumentId", c => c.Int(nullable: false));
            CreateIndex("dbo.TemplateDocumentSendListsSet", "TemplateDocuments_Id");
        }
    }
}
