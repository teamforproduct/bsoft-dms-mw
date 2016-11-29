namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SystemUIElements : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.SystemUIElements", "Order", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("DMS.SystemUIElements", "Order");
        }
    }
}
