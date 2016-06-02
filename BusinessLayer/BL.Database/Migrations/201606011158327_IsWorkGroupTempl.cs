namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsWorkGroupTempl : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.TemplateDocumentSendLists", "IsWorkGroup", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("DMS.TemplateDocumentSendLists", "IsWorkGroup");
        }
    }
}
