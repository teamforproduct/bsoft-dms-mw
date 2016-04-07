namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RedesignPaperEvents3 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "DMS.DocumentPapers", newName: "DocumentPapersTMPs");
        }
        
        public override void Down()
        {
            RenameTable(name: "DMS.DocumentPapersTMPs", newName: "DocumentPapers");
        }
    }
}
