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
            AddColumn("dbo.DocumentSendLists", "TaskName", c => c.String());
            AddColumn("dbo.Waits", "TaskName", c => c.String());
            DropColumn("dbo.Waits", "Description");
            DropColumn("dbo.DictionaryDocumentTypes", "DirectionCodes");
            DropColumn("dbo.TemplateDocumentSendListsSet", "DueDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TemplateDocumentSendListsSet", "DueDate", c => c.DateTime());
            AddColumn("dbo.DictionaryDocumentTypes", "DirectionCodes", c => c.String());
            AddColumn("dbo.Waits", "Description", c => c.String());
            DropColumn("dbo.Waits", "TaskName");
            DropColumn("dbo.DocumentSendLists", "TaskName");
            DropColumn("dbo.Documents", "IsLaunchPlan");
            DropColumn("dbo.SystemActions", "IsVisible");
        }
    }
}
