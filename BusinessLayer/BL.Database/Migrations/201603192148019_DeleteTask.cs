namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteTask : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.DocumentEvents", "Task");
            DropColumn("dbo.DocumentSendLists", "Task");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DocumentSendLists", "Task", c => c.String());
            AddColumn("dbo.DocumentEvents", "Task", c => c.String());
        }
    }
}
