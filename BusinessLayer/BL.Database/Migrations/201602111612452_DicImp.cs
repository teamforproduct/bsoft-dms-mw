namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DicImp : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.DictionaryEventTypes", name: "ImpotanceEventTypeId", newName: "ImportanceEventTypeId");
            RenameIndex(table: "dbo.DictionaryEventTypes", name: "IX_ImpotanceEventTypeId", newName: "IX_ImportanceEventTypeId");
            AddColumn("dbo.DictionarySendTypes", "IsImportant", c => c.Boolean(nullable: false));
            AddColumn("dbo.DictionaryLinkTypes", "IsImportant", c => c.Boolean(nullable: false));
            DropColumn("dbo.DictionarySendTypes", "IsImpotant");
            DropColumn("dbo.DictionaryLinkTypes", "IsImpotant");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DictionaryLinkTypes", "IsImpotant", c => c.Boolean(nullable: false));
            AddColumn("dbo.DictionarySendTypes", "IsImpotant", c => c.Boolean(nullable: false));
            DropColumn("dbo.DictionaryLinkTypes", "IsImportant");
            DropColumn("dbo.DictionarySendTypes", "IsImportant");
            RenameIndex(table: "dbo.DictionaryEventTypes", name: "IX_ImportanceEventTypeId", newName: "IX_ImpotanceEventTypeId");
            RenameColumn(table: "dbo.DictionaryEventTypes", name: "ImportanceEventTypeId", newName: "ImpotanceEventTypeId");
        }
    }
}
