namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeSendLists : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DictionaryStandartSendListContents", "Stage", c => c.Int(nullable: false));
            AddColumn("dbo.DocumentSendLists", "Stage", c => c.Int(nullable: false));
            AddColumn("dbo.TemplateDocumentSendLists", "Stage", c => c.Int(nullable: false));
            DropColumn("dbo.DictionaryStandartSendListContents", "OrderNumber");
            DropColumn("dbo.DocumentSendLists", "OrderNumber");
            DropColumn("dbo.TemplateDocumentSendLists", "OrderNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TemplateDocumentSendLists", "OrderNumber", c => c.Int());
            AddColumn("dbo.DocumentSendLists", "OrderNumber", c => c.Int());
            AddColumn("dbo.DictionaryStandartSendListContents", "OrderNumber", c => c.Int(nullable: false));
            DropColumn("dbo.TemplateDocumentSendLists", "Stage");
            DropColumn("dbo.DocumentSendLists", "Stage");
            DropColumn("dbo.DictionaryStandartSendListContents", "Stage");
        }
    }
}
