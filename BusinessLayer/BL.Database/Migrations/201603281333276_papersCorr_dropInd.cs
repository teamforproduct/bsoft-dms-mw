namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class papersCorr_dropInd : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("DMS.DocumentPaperEvents", "SendListId", "DMS.DictionarySendTypes");
            DropForeignKey("DMS.DocumentPaperEvents", "DocumentSendLists_Id", "DMS.DocumentSendLists");
            DropIndex("DMS.DocumentPaperEvents", new[] { "SendListId" });
            DropIndex("DMS.DocumentPaperEvents", new[] { "DocumentSendLists_Id" });
            DropColumn("DMS.DocumentPaperEvents", "DocumentSendLists_Id");
        }
        
        public override void Down()
        {
            AddColumn("DMS.DocumentPaperEvents", "DocumentSendLists_Id", c => c.Int());
            CreateIndex("DMS.DocumentPaperEvents", "DocumentSendLists_Id");
            CreateIndex("DMS.DocumentPaperEvents", "SendListId");
            AddForeignKey("DMS.DocumentPaperEvents", "DocumentSendLists_Id", "DMS.DocumentSendLists", "Id");
            AddForeignKey("DMS.DocumentPaperEvents", "SendListId", "DMS.DictionarySendTypes", "Id");
        }
    }
}
