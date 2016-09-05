namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class indexEvent : DbMigration
    {
        public override void Up()
        {
            DropIndex("DMS.DocumentEvents", new[] { "LastChangeDate" });
            RenameIndex(table: "DMS.DocumentEvents", name: "IX_DocumentEvents_ReadDate", newName: "IX_ReadDate");
            RenameIndex(table: "DMS.DocumentEvents", name: "IX_DocumentEvents_IsAvailableWithinTask", newName: "IX_IsAvailableWithinTask");
            CreateIndex("DMS.DocumentEvents", "DocumentId");
            CreateIndex("DMS.DocumentEvents", "TaskId");
            CreateIndex("DMS.DocumentEvents", "LastChangeDate");
        }
        
        public override void Down()
        {
            DropIndex("DMS.DocumentEvents", new[] { "LastChangeDate" });
            DropIndex("DMS.DocumentEvents", new[] { "TaskId" });
            DropIndex("DMS.DocumentEvents", new[] { "DocumentId" });
            RenameIndex(table: "DMS.DocumentEvents", name: "IX_IsAvailableWithinTask", newName: "IX_DocumentEvents_IsAvailableWithinTask");
            RenameIndex(table: "DMS.DocumentEvents", name: "IX_ReadDate", newName: "IX_DocumentEvents_ReadDate");
            CreateIndex("DMS.DocumentEvents", "LastChangeDate");
        }
    }
}
