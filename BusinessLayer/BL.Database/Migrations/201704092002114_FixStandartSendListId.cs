namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixStandartSendListId : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.DocumentSendListAccessGroups", "StandartSendListId", c => c.Int());
            AddColumn("DMS.DocumentEventAccessGroups", "StandartSendListId", c => c.Int());
            AddColumn("DMS.TemplateDocumentSendListAccessGroups", "StandartSendListId", c => c.Int());
            CreateIndex("DMS.DocumentSendListAccessGroups", "StandartSendListId");
            CreateIndex("DMS.DocumentEventAccessGroups", "StandartSendListId");
            CreateIndex("DMS.TemplateDocumentSendListAccessGroups", "StandartSendListId");
            AddForeignKey("DMS.DocumentSendListAccessGroups", "StandartSendListId", "DMS.DictionaryStandartSendLists", "Id");
            AddForeignKey("DMS.DocumentEventAccessGroups", "StandartSendListId", "DMS.DictionaryStandartSendLists", "Id");
            AddForeignKey("DMS.TemplateDocumentSendListAccessGroups", "StandartSendListId", "DMS.DictionaryStandartSendLists", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.TemplateDocumentSendListAccessGroups", "StandartSendListId", "DMS.DictionaryStandartSendLists");
            DropForeignKey("DMS.DocumentEventAccessGroups", "StandartSendListId", "DMS.DictionaryStandartSendLists");
            DropForeignKey("DMS.DocumentSendListAccessGroups", "StandartSendListId", "DMS.DictionaryStandartSendLists");
            DropIndex("DMS.TemplateDocumentSendListAccessGroups", new[] { "StandartSendListId" });
            DropIndex("DMS.DocumentEventAccessGroups", new[] { "StandartSendListId" });
            DropIndex("DMS.DocumentSendListAccessGroups", new[] { "StandartSendListId" });
            DropColumn("DMS.TemplateDocumentSendListAccessGroups", "StandartSendListId");
            DropColumn("DMS.DocumentEventAccessGroups", "StandartSendListId");
            DropColumn("DMS.DocumentSendListAccessGroups", "StandartSendListId");
        }
    }
}
