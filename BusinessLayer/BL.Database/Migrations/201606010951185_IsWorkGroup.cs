namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsWorkGroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.Documents", "AddDescription", c => c.String());
            AddColumn("DMS.DocumentSendLists", "AddDescription", c => c.String());
            AddColumn("DMS.DocumentSendLists", "IsWorkGroup", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("DMS.DocumentSendLists", "IsWorkGroup");
            DropColumn("DMS.DocumentSendLists", "AddDescription");
            DropColumn("DMS.Documents", "AddDescription");
        }
    }
}
