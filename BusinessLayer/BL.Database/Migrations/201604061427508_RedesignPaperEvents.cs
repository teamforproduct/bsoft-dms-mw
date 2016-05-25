namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RedesignPaperEvents : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("DMS.DocumentPaperEvents", "PaperId", "DMS.DocumentPapers");
            DropIndex("DMS.DocumentEvents", new[] { "SourceAgentId" });
            AddColumn("DMS.DocumentEvents", "PaperId", c => c.Int());
            AddColumn("DMS.DocumentEvents", "SendListId", c => c.Int());
            AddColumn("DMS.DocumentEvents", "ParentEventId", c => c.Int());
            AddColumn("DMS.DocumentEvents", "PaperListId", c => c.Int());
            AddColumn("DMS.DocumentEvents", "PaperPlanAgentId", c => c.Int());
            AddColumn("DMS.DocumentEvents", "PaperPlanDate", c => c.DateTime());
            AddColumn("DMS.DocumentEvents", "PaperSendAgentId", c => c.Int());
            AddColumn("DMS.DocumentEvents", "PaperSendDate", c => c.DateTime());
            AddColumn("DMS.DocumentEvents", "PaperRecieveAgentId", c => c.Int());
            AddColumn("DMS.DocumentEvents", "PaperRecieveDate", c => c.DateTime());
            AlterColumn("DMS.DocumentEvents", "SourceAgentId", c => c.Int());
            CreateIndex("DMS.DocumentEvents", "SourceAgentId");
            CreateIndex("DMS.DocumentEvents", "PaperId");
            CreateIndex("DMS.DocumentEvents", "SendListId");
            CreateIndex("DMS.DocumentEvents", "ParentEventId");
            CreateIndex("DMS.DocumentEvents", "PaperListId");
            CreateIndex("DMS.DocumentEvents", "PaperPlanAgentId");
            CreateIndex("DMS.DocumentEvents", "PaperSendAgentId");
            CreateIndex("DMS.DocumentEvents", "PaperRecieveAgentId");
            AddForeignKey("DMS.DocumentEvents", "ParentEventId", "DMS.DocumentEvents", "Id");
            AddForeignKey("DMS.DocumentEvents", "PaperId", "DMS.DocumentPapers", "Id");
            AddForeignKey("DMS.DocumentEvents", "PaperListId", "DMS.DocumentPaperLists", "Id");
            AddForeignKey("DMS.DocumentEvents", "PaperPlanAgentId", "DMS.DictionaryAgents", "Id");
            AddForeignKey("DMS.DocumentEvents", "PaperRecieveAgentId", "DMS.DictionaryAgents", "Id");
            AddForeignKey("DMS.DocumentEvents", "PaperSendAgentId", "DMS.DictionaryAgents", "Id");
            AddForeignKey("DMS.DocumentEvents", "SendListId", "DMS.DocumentSendLists", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.DocumentEvents", "SendListId", "DMS.DocumentSendLists");
            DropForeignKey("DMS.DocumentEvents", "PaperSendAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentEvents", "PaperRecieveAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentEvents", "PaperPlanAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentEvents", "PaperListId", "DMS.DocumentPaperLists");
            DropForeignKey("DMS.DocumentEvents", "PaperId", "DMS.DocumentPapers");
            DropForeignKey("DMS.DocumentEvents", "ParentEventId", "DMS.DocumentEvents");
            DropIndex("DMS.DocumentEvents", new[] { "PaperRecieveAgentId" });
            DropIndex("DMS.DocumentEvents", new[] { "PaperSendAgentId" });
            DropIndex("DMS.DocumentEvents", new[] { "PaperPlanAgentId" });
            DropIndex("DMS.DocumentEvents", new[] { "PaperListId" });
            DropIndex("DMS.DocumentEvents", new[] { "ParentEventId" });
            DropIndex("DMS.DocumentEvents", new[] { "SendListId" });
            DropIndex("DMS.DocumentEvents", new[] { "PaperId" });
            DropIndex("DMS.DocumentEvents", new[] { "SourceAgentId" });
            AlterColumn("DMS.DocumentEvents", "SourceAgentId", c => c.Int(nullable: false));
            DropColumn("DMS.DocumentEvents", "PaperRecieveDate");
            DropColumn("DMS.DocumentEvents", "PaperRecieveAgentId");
            DropColumn("DMS.DocumentEvents", "PaperSendDate");
            DropColumn("DMS.DocumentEvents", "PaperSendAgentId");
            DropColumn("DMS.DocumentEvents", "PaperPlanDate");
            DropColumn("DMS.DocumentEvents", "PaperPlanAgentId");
            DropColumn("DMS.DocumentEvents", "PaperListId");
            DropColumn("DMS.DocumentEvents", "ParentEventId");
            DropColumn("DMS.DocumentEvents", "SendListId");
            DropColumn("DMS.DocumentEvents", "PaperId");
            CreateIndex("DMS.DocumentEvents", "SourceAgentId");
            AddForeignKey("DMS.DocumentPaperEvents", "PaperId", "DMS.DocumentPapers", "Id");
        }
    }
}
