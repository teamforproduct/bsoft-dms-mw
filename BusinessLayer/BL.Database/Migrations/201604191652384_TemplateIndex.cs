namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TemplateIndex : DbMigration
    {
        public override void Up()
        {
            DropIndex("DMS.TemplateDocumentFiles", new[] { "DocumentId" });
            DropIndex("DMS.TempDocRestrictedSendLists", new[] { "DocumentId" });
            DropIndex("DMS.TempDocRestrictedSendLists", new[] { "PositionId" });
            DropIndex("DMS.TemplateDocumentTasks", new[] { "DocumentId" });
            CreateIndex("DMS.TemplateDocuments", "Name", unique: true);
            CreateIndex("DMS.TemplateDocumentFiles", new[] { "DocumentId", "Name", "Extention" }, unique: true, name: "IX_DocumentNameExtention");
            CreateIndex("DMS.TemplateDocumentFiles", new[] { "DocumentId", "OrderNumber" }, unique: true, name: "IX_DocumentOrderNumber");
            CreateIndex("DMS.TempDocRestrictedSendLists", new[] { "DocumentId", "PositionId" }, unique: true, name: "IX_DocumentPosition");
            CreateIndex("DMS.TempDocRestrictedSendLists", "PositionId");
            CreateIndex("DMS.TemplateDocumentTasks", new[] { "DocumentId", "Task" }, unique: true, name: "IX_DocumentTask");
            DropColumn("DMS.TemplateDocumentFiles", "Content");
        }
        
        public override void Down()
        {
            AddColumn("DMS.TemplateDocumentFiles", "Content", c => c.Binary());
            DropIndex("DMS.TemplateDocumentTasks", "IX_DocumentTask");
            DropIndex("DMS.TempDocRestrictedSendLists", new[] { "PositionId" });
            DropIndex("DMS.TempDocRestrictedSendLists", "IX_DocumentPosition");
            DropIndex("DMS.TemplateDocumentFiles", "IX_DocumentOrderNumber");
            DropIndex("DMS.TemplateDocumentFiles", "IX_DocumentNameExtention");
            DropIndex("DMS.TemplateDocuments", new[] { "Name" });
            CreateIndex("DMS.TemplateDocumentTasks", "DocumentId");
            CreateIndex("DMS.TempDocRestrictedSendLists", "PositionId");
            CreateIndex("DMS.TempDocRestrictedSendLists", "DocumentId");
            CreateIndex("DMS.TemplateDocumentFiles", "DocumentId");
        }
    }
}
