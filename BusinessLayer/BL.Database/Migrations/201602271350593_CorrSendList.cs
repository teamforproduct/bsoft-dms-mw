namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CorrSendList : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.DocumentEvents", new[] { "SourcePositionId" });
            AddColumn("dbo.DictionaryStandartSendListContents", "TargetAgentId", c => c.Int());
            AddColumn("dbo.DocumentSendLists", "SourcePositionId", c => c.Int());
            AddColumn("dbo.DocumentSendLists", "SourceAgentId", c => c.Int(nullable: false));
            AddColumn("dbo.DocumentSendLists", "TargetAgentId", c => c.Int());
            AddColumn("dbo.TemplateDocumentSendListsSet", "SourcePositionId", c => c.Int());
            AddColumn("dbo.TemplateDocumentSendListsSet", "TargetAgentId", c => c.Int());
            AlterColumn("dbo.DocumentEvents", "SourcePositionId", c => c.Int());
            CreateIndex("dbo.DictionaryStandartSendListContents", "TargetAgentId");
            CreateIndex("dbo.DocumentEvents", "SourcePositionId");
            CreateIndex("dbo.DocumentSendLists", "SourcePositionId");
            CreateIndex("dbo.DocumentSendLists", "SourceAgentId");
            CreateIndex("dbo.DocumentSendLists", "TargetAgentId");
            CreateIndex("dbo.TemplateDocumentSendListsSet", "SourcePositionId");
            CreateIndex("dbo.TemplateDocumentSendListsSet", "TargetAgentId");
            AddForeignKey("dbo.DictionaryStandartSendListContents", "TargetAgentId", "dbo.DictionaryAgents", "Id");
            AddForeignKey("dbo.DocumentSendLists", "SourceAgentId", "dbo.DictionaryAgents", "Id");
            AddForeignKey("dbo.DocumentSendLists", "SourcePositionId", "dbo.DictionaryPositions", "Id");
            AddForeignKey("dbo.DocumentSendLists", "TargetAgentId", "dbo.DictionaryAgents", "Id");
            AddForeignKey("dbo.TemplateDocumentSendListsSet", "SourcePositionId", "dbo.DictionaryAgents", "Id");
            AddForeignKey("dbo.TemplateDocumentSendListsSet", "TargetAgentId", "dbo.DictionaryAgents", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TemplateDocumentSendListsSet", "TargetAgentId", "dbo.DictionaryAgents");
            DropForeignKey("dbo.TemplateDocumentSendListsSet", "SourcePositionId", "dbo.DictionaryAgents");
            DropForeignKey("dbo.DocumentSendLists", "TargetAgentId", "dbo.DictionaryAgents");
            DropForeignKey("dbo.DocumentSendLists", "SourcePositionId", "dbo.DictionaryPositions");
            DropForeignKey("dbo.DocumentSendLists", "SourceAgentId", "dbo.DictionaryAgents");
            DropForeignKey("dbo.DictionaryStandartSendListContents", "TargetAgentId", "dbo.DictionaryAgents");
            DropIndex("dbo.TemplateDocumentSendListsSet", new[] { "TargetAgentId" });
            DropIndex("dbo.TemplateDocumentSendListsSet", new[] { "SourcePositionId" });
            DropIndex("dbo.DocumentSendLists", new[] { "TargetAgentId" });
            DropIndex("dbo.DocumentSendLists", new[] { "SourceAgentId" });
            DropIndex("dbo.DocumentSendLists", new[] { "SourcePositionId" });
            DropIndex("dbo.DocumentEvents", new[] { "SourcePositionId" });
            DropIndex("dbo.DictionaryStandartSendListContents", new[] { "TargetAgentId" });
            AlterColumn("dbo.DocumentEvents", "SourcePositionId", c => c.Int(nullable: false));
            DropColumn("dbo.TemplateDocumentSendListsSet", "TargetAgentId");
            DropColumn("dbo.TemplateDocumentSendListsSet", "SourcePositionId");
            DropColumn("dbo.DocumentSendLists", "TargetAgentId");
            DropColumn("dbo.DocumentSendLists", "SourceAgentId");
            DropColumn("dbo.DocumentSendLists", "SourcePositionId");
            DropColumn("dbo.DictionaryStandartSendListContents", "TargetAgentId");
            CreateIndex("dbo.DocumentEvents", "SourcePositionId");
        }
    }
}
