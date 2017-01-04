namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PlanDueDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.DocumentWaits", "PlanDueDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("DMS.DocumentWaits", "PlanDueDate");
        }
    }
}
