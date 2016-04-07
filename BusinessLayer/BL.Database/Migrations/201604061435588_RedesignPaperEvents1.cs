namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RedesignPaperEvents1 : DbMigration
    {
        public override void Up()
        {
            DropIndex("DMS.DocumentPapers", new[] { "LastPaperEventId" });
            DropColumn("DMS.DocumentPapers", "LastChangeUserId");
            RenameColumn(table: "DMS.DocumentPapers", name: "LastPaperEventId", newName: "LastChangeUserId");
            AlterColumn("DMS.DocumentPapers", "LastChangeUserId", c => c.Int(nullable: false));
            CreateIndex("DMS.DocumentPapers", "LastChangeUserId");
        }
        
        public override void Down()
        {
            DropIndex("DMS.DocumentPapers", new[] { "LastChangeUserId" });
            AlterColumn("DMS.DocumentPapers", "LastChangeUserId", c => c.Int());
            RenameColumn(table: "DMS.DocumentPapers", name: "LastChangeUserId", newName: "LastPaperEventId");
            AddColumn("DMS.DocumentPapers", "LastChangeUserId", c => c.Int(nullable: false));
            CreateIndex("DMS.DocumentPapers", "LastPaperEventId");
        }
    }
}
