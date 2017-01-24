namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifySendList : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("DMS.TemplateDocumentSendLists", "SourcePositionId", "DMS.DictionaryAgents");
            DropIndex("DMS.TemplateDocumentSendLists", new[] { "SourcePositionId" });
            AddColumn("DMS.DocumentSendLists", "SelfAttentionDay", c => c.Int());
            AddColumn("DMS.TemplateDocumentSendLists", "SelfAttentionDay", c => c.Int());
            DropColumn("DMS.TemplateDocumentSendLists", "SourcePositionId");
            DropColumn("DMS.TemplateDocumentSendLists", "SelfDueDate");
            DropColumn("DMS.TemplateDocumentSendLists", "SelfAttentionDate");
        }
        
        public override void Down()
        {
            AddColumn("DMS.TemplateDocumentSendLists", "SelfAttentionDate", c => c.DateTime());
            AddColumn("DMS.TemplateDocumentSendLists", "SelfDueDate", c => c.DateTime());
            AddColumn("DMS.TemplateDocumentSendLists", "SourcePositionId", c => c.Int());
            DropColumn("DMS.TemplateDocumentSendLists", "SelfAttentionDay");
            DropColumn("DMS.DocumentSendLists", "SelfAttentionDay");
            CreateIndex("DMS.TemplateDocumentSendLists", "SourcePositionId");
            AddForeignKey("DMS.TemplateDocumentSendLists", "SourcePositionId", "DMS.DictionaryAgents", "Id");
        }
    }
}
