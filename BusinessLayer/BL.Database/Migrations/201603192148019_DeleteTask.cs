namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteTask : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.DocumentEvents", "TaskName");
            DropColumn("dbo.DocumentSendLists", "TaskName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DocumentSendLists", "TaskName", c => c.String());
            AddColumn("dbo.DocumentEvents", "TaskName", c => c.String());
        }
    }
}
