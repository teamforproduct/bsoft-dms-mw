namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class corr : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SystemActions", "IsVisible", c => c.Boolean(nullable: false));
            AddColumn("dbo.Documents", "IsLaunchPlan", c => c.Boolean(nullable: false));
            AddColumn("dbo.DocumentSendLists", "Task", c => c.String());
            AddColumn("dbo.DocumentWaits", "Task", c => c.String());
            DropColumn("dbo.DocumentWaits", "Description");
            DropColumn("dbo.DictionaryDocumentTypes", "DirectionCodes");
            DropColumn("dbo.TemplateDocumentSendLists", "DueDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TemplateDocumentSendLists", "DueDate", c => c.DateTime());
            AddColumn("dbo.DictionaryDocumentTypes", "DirectionCodes", c => c.String());
            AddColumn("dbo.DocumentWaits", "Description", c => c.String());
            DropColumn("dbo.DocumentWaits", "Task");
            DropColumn("dbo.DocumentSendLists", "Task");
            DropColumn("dbo.Documents", "IsLaunchPlan");
            DropColumn("dbo.SystemActions", "IsVisible");
        }
    }
}
