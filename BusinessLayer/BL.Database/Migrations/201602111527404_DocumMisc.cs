namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DocumMisc : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Waits", new[] { "OnEventId" });
            DropIndex("dbo.Waits", new[] { "DocumentEvents_Id" });
            DropIndex("dbo.Waits", new[] { "DocumentEvents_Id1" });
            DropColumn("dbo.Waits", "OffEventId");
            DropColumn("dbo.Waits", "OnEventId");
            RenameColumn(table: "dbo.Waits", name: "DocumentEvents_Id", newName: "OffEventId");
            RenameColumn(table: "dbo.Waits", name: "DocumentEvents_Id1", newName: "OnEventId");
            RenameColumn(table: "dbo.DocumentSendLists", name: "EventId", newName: "CloseEventId");
            RenameIndex(table: "dbo.DocumentSendLists", name: "IX_EventId", newName: "IX_CloseEventId");
            AddColumn("dbo.DictionaryRegistrationJournals", "NumerationPrefixFormula", c => c.String());
            AddColumn("dbo.Documents", "NumerationPrefixFormula", c => c.String());
            AddColumn("dbo.Documents", "LinkId", c => c.Int());
            AddColumn("dbo.DocumentSendLists", "StartEventId", c => c.Int());
            AlterColumn("dbo.Waits", "OnEventId", c => c.Int(nullable: false));
            CreateIndex("dbo.DocumentSendLists", "StartEventId");
            CreateIndex("dbo.Waits", "OnEventId");
            AddForeignKey("dbo.DocumentSendLists", "StartEventId", "dbo.DocumentEvents", "Id");
            AddForeignKey("dbo.DocumentSendLists", "CloseEventId", "dbo.DocumentEvents", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DocumentSendLists", "CloseEventId", "dbo.DocumentEvents");
            DropForeignKey("dbo.DocumentSendLists", "StartEventId", "dbo.DocumentEvents");
            DropIndex("dbo.Waits", new[] { "OnEventId" });
            DropIndex("dbo.DocumentSendLists", new[] { "StartEventId" });
            AlterColumn("dbo.Waits", "OnEventId", c => c.Int());
            DropColumn("dbo.DocumentSendLists", "StartEventId");
            DropColumn("dbo.Documents", "LinkId");
            DropColumn("dbo.Documents", "NumerationPrefixFormula");
            DropColumn("dbo.DictionaryRegistrationJournals", "NumerationPrefixFormula");
            RenameIndex(table: "dbo.DocumentSendLists", name: "IX_CloseEventId", newName: "IX_EventId");
            RenameColumn(table: "dbo.DocumentSendLists", name: "CloseEventId", newName: "EventId");
            RenameColumn(table: "dbo.Waits", name: "OnEventId", newName: "DocumentEvents_Id1");
            RenameColumn(table: "dbo.Waits", name: "OffEventId", newName: "DocumentEvents_Id");
            AddColumn("dbo.Waits", "OnEventId", c => c.Int(nullable: false));
            AddColumn("dbo.Waits", "OffEventId", c => c.Int());
            CreateIndex("dbo.Waits", "DocumentEvents_Id1");
            CreateIndex("dbo.Waits", "DocumentEvents_Id");
            CreateIndex("dbo.Waits", "OnEventId");
        }
    }
}
