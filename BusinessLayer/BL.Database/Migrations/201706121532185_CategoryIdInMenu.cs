namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CategoryIdInMenu : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.SystemMenus", "CategoryId", c => c.Int());
            AlterColumn("DMS.SystemMenus", "Order", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("DMS.SystemMenus", "Order", c => c.Int(nullable: false));
            DropColumn("DMS.SystemMenus", "CategoryId");
        }
    }
}
