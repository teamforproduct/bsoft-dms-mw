namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DDictEvImp : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.DictionaryImpotanceEventTypes", newName: "DictionaryImportanceEventTypes");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.DictionaryImportanceEventTypes", newName: "DictionaryImpotanceEventTypes");
        }
    }
}
