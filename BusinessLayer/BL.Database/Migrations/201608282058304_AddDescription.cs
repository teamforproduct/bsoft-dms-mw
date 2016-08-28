namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDescription : DbMigration
    {
        public override void Up()
        {
            AlterColumn("DMS.DocumentEvents", "AddDescription", c => c.String(maxLength: 2000));
            AlterColumn("DMS.DocumentSendLists", "AddDescription", c => c.String(maxLength: 2000));
            AlterColumn("DMS.DocumentFiles", "Content", c => c.String(maxLength: 2000));
        }
        
        public override void Down()
        {
            AlterColumn("DMS.DocumentFiles", "Content", c => c.String());
            AlterColumn("DMS.DocumentSendLists", "AddDescription", c => c.String());
            AlterColumn("DMS.DocumentEvents", "AddDescription", c => c.String());
        }
    }
}
