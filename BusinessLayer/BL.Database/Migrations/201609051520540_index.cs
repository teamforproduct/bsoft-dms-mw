namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class index : DbMigration
    {
        public override void Up()
        {
            DropIndex("DMS.Documents", new[] { "CreateDate" });
            DropIndex("DMS.DocumentWaits", new[] { "DueDate" });
            DropIndex("DMS.DocumentFiles", new[] { "LastChangeDate" });
            CreateIndex("DMS.Documents", "TemplateDocumentId");
            CreateIndex("DMS.Documents", "CreateDate");
            CreateIndex("DMS.DocumentWaits", "DueDate");
            CreateIndex("DMS.DocumentFiles", "LastChangeDate");
            CreateIndex("DMS.DictionaryRegistrationJournals", "DepartmentId");
            CreateIndex("DMS.PropertyValues", new[] { "RecordId", "PropertyLinkId" }, unique: true, name: "IX_RecordId");
        }
        
        public override void Down()
        {
            DropIndex("DMS.PropertyValues", "IX_RecordId");
            DropIndex("DMS.DictionaryRegistrationJournals", new[] { "DepartmentId" });
            DropIndex("DMS.DocumentFiles", new[] { "LastChangeDate" });
            DropIndex("DMS.DocumentWaits", new[] { "DueDate" });
            DropIndex("DMS.Documents", new[] { "CreateDate" });
            DropIndex("DMS.Documents", new[] { "TemplateDocumentId" });
            CreateIndex("DMS.DocumentFiles", "LastChangeDate");
            CreateIndex("DMS.DocumentWaits", "DueDate");
            CreateIndex("DMS.Documents", "CreateDate");
        }
    }
}
