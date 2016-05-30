namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BLCorrections : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.DocumentEvents", "AddDescription", c => c.String());
            AddColumn("DMS.DocumentSendLists", "SelfDueDate", c => c.DateTime());
            AddColumn("DMS.DocumentSendLists", "SelfDueDay", c => c.Int());
            AddColumn("DMS.DocumentSendLists", "SelfAttentionDate", c => c.DateTime());
            AddColumn("DMS.TemplateDocumentSendLists", "SelfDueDate", c => c.DateTime());
            AddColumn("DMS.TemplateDocumentSendLists", "SelfDueDay", c => c.Int());
            AddColumn("DMS.TemplateDocumentSendLists", "SelfAttentionDate", c => c.DateTime());
            DropColumn("DMS.DocumentWaits", "TargetAttentionDate");
        }
        
        public override void Down()
        {
            AddColumn("DMS.DocumentWaits", "TargetAttentionDate", c => c.DateTime());
            DropColumn("DMS.TemplateDocumentSendLists", "SelfAttentionDate");
            DropColumn("DMS.TemplateDocumentSendLists", "SelfDueDay");
            DropColumn("DMS.TemplateDocumentSendLists", "SelfDueDate");
            DropColumn("DMS.DocumentSendLists", "SelfAttentionDate");
            DropColumn("DMS.DocumentSendLists", "SelfDueDay");
            DropColumn("DMS.DocumentSendLists", "SelfDueDate");
            DropColumn("DMS.DocumentEvents", "AddDescription");
        }
    }
}
