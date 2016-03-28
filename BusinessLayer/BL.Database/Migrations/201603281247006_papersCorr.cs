namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class papersCorr : DbMigration
    {
        public override void Up()
        {
            DropIndex("DMS.DocumentPaperEvents", new[] { "PlanAgentId" });
            AddColumn("DMS.DocumentPaperEvents", "SendListId", c => c.Int());
            AddColumn("DMS.DocumentPaperEvents", "DocumentSendLists_Id", c => c.Int());
            AddColumn("DMS.SystemActions", "Category", c => c.String(maxLength: 2000));
            AddColumn("DMS.AdminUserRoles", "StartDate", c => c.DateTime(nullable: false));
            AddColumn("DMS.AdminUserRoles", "EndDate", c => c.DateTime(nullable: false));
            AlterColumn("DMS.DocumentPaperEvents", "Description", c => c.String(maxLength: 2000));
            AlterColumn("DMS.DocumentPaperEvents", "PlanAgentId", c => c.Int());
            AlterColumn("DMS.DocumentPaperEvents", "PlanDate", c => c.DateTime());
            AlterColumn("DMS.DocumentPapers", "Name", c => c.String(maxLength: 2000));
            AlterColumn("DMS.DocumentPapers", "Description", c => c.String(maxLength: 2000));
            AlterColumn("DMS.DocumentPapers", "LastPaperEventId", c => c.Int());
            AlterColumn("DMS.DocumentPaperLists", "Description", c => c.String(maxLength: 2000));
            CreateIndex("DMS.DocumentPaperEvents", "SendListId");
            CreateIndex("DMS.DocumentPaperEvents", "PlanAgentId");
            CreateIndex("DMS.DocumentPaperEvents", "DocumentSendLists_Id");
            CreateIndex("DMS.DocumentPapers", "LastPaperEventId");
            AddForeignKey("DMS.DocumentPapers", "LastPaperEventId", "DMS.DocumentPaperEvents", "Id");
            AddForeignKey("DMS.DocumentPaperEvents", "SendListId", "DMS.DictionarySendTypes", "Id");
            AddForeignKey("DMS.DocumentPaperEvents", "DocumentSendLists_Id", "DMS.DocumentSendLists", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.DocumentPaperEvents", "DocumentSendLists_Id", "DMS.DocumentSendLists");
            DropForeignKey("DMS.DocumentPaperEvents", "SendListId", "DMS.DictionarySendTypes");
            DropForeignKey("DMS.DocumentPapers", "LastPaperEventId", "DMS.DocumentPaperEvents");
            DropIndex("DMS.DocumentPapers", new[] { "LastPaperEventId" });
            DropIndex("DMS.DocumentPaperEvents", new[] { "DocumentSendLists_Id" });
            DropIndex("DMS.DocumentPaperEvents", new[] { "PlanAgentId" });
            DropIndex("DMS.DocumentPaperEvents", new[] { "SendListId" });
            AlterColumn("DMS.DocumentPaperLists", "Description", c => c.String());
            AlterColumn("DMS.DocumentPapers", "LastPaperEventId", c => c.Int(nullable: false));
            AlterColumn("DMS.DocumentPapers", "Description", c => c.String());
            AlterColumn("DMS.DocumentPapers", "Name", c => c.String());
            AlterColumn("DMS.DocumentPaperEvents", "PlanDate", c => c.DateTime(nullable: false));
            AlterColumn("DMS.DocumentPaperEvents", "PlanAgentId", c => c.Int(nullable: false));
            AlterColumn("DMS.DocumentPaperEvents", "Description", c => c.String());
            DropColumn("DMS.AdminUserRoles", "EndDate");
            DropColumn("DMS.AdminUserRoles", "StartDate");
            DropColumn("DMS.SystemActions", "Category");
            DropColumn("DMS.DocumentPaperEvents", "DocumentSendLists_Id");
            DropColumn("DMS.DocumentPaperEvents", "SendListId");
            CreateIndex("DMS.DocumentPaperEvents", "PlanAgentId");
        }
    }
}
