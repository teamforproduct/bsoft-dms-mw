namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DictionarySendType_AddOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.DictionarySendTypes", "Order", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("DMS.DictionarySendTypes", "Order");
        }
    }
}
