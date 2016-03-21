namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFieldTask : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DictionaryStandartSendListContents", "TaskName", c => c.String());
            AddColumn("dbo.TemplateDocumentSendListsSet", "TaskName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TemplateDocumentSendListsSet", "TaskName");
            DropColumn("dbo.DictionaryStandartSendListContents", "TaskName");
        }
    }
}
