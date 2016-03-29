namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class papersCorr_addInd : DbMigration
    {
        public override void Up()
        {
            CreateIndex("DMS.DocumentPaperEvents", "SendListId");
            AddForeignKey("DMS.DocumentPaperEvents", "SendListId", "DMS.DocumentSendLists", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.DocumentPaperEvents", "SendListId", "DMS.DocumentSendLists");
            DropIndex("DMS.DocumentPaperEvents", new[] { "SendListId" });
        }
    }
}
