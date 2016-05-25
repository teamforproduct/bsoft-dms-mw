namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RedesignPaperEvents7 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("DMS.DocumentEvents", "PaperId", "DMS.DocumentPapersTMPs");
            AddForeignKey("DMS.DocumentEvents", "PaperId", "DMS.DocumentPapers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.DocumentEvents", "PaperId", "DMS.DocumentPapers");
            AddForeignKey("DMS.DocumentEvents", "PaperId", "DMS.DocumentPapersTMPs", "Id");
        }
    }
}
