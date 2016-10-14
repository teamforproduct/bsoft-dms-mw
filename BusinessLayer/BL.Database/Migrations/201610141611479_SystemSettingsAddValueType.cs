namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SystemSettingsAddValueType : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.SystemSettings", "ValueType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("DMS.SystemSettings", "ValueType");
        }
    }
}
