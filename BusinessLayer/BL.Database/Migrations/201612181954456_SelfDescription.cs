namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SelfDescription : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.DocumentSendLists", "SelfDescription", c => c.String(maxLength: 2000));
            AddColumn("DMS.TemplateDocumentSendLists", "SelfDescription", c => c.String(maxLength: 2000));
        }
        
        public override void Down()
        {
            DropColumn("DMS.TemplateDocumentSendLists", "SelfDescription");
            DropColumn("DMS.DocumentSendLists", "SelfDescription");
        }
    }
}
