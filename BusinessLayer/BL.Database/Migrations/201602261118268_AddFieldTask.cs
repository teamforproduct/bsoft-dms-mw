namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFieldTask : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DictionaryStandartSendListContents", "Task", c => c.String());
            AddColumn("dbo.TemplateDocumentSendLists", "Task", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TemplateDocumentSendLists", "Task");
            DropColumn("dbo.DictionaryStandartSendListContents", "Task");
        }
    }
}
