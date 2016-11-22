namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SystemSettings2 : DbMigration
    {
        public override void Up()
        {
            DropIndex("DMS.SystemSettings", new[] { "ValueTypeId" });
            DropIndex("DMS.SystemSettings", new[] { "SettingTypeId" });
            AlterColumn("DMS.SystemSettings", "ValueTypeId", c => c.Int(nullable: false));
            AlterColumn("DMS.SystemSettings", "SettingTypeId", c => c.Int(nullable: false));
            CreateIndex("DMS.SystemSettings", "ValueTypeId");
            CreateIndex("DMS.SystemSettings", "SettingTypeId");
        }
        
        public override void Down()
        {
            DropIndex("DMS.SystemSettings", new[] { "SettingTypeId" });
            DropIndex("DMS.SystemSettings", new[] { "ValueTypeId" });
            AlterColumn("DMS.SystemSettings", "SettingTypeId", c => c.Int());
            AlterColumn("DMS.SystemSettings", "ValueTypeId", c => c.Int());
            CreateIndex("DMS.SystemSettings", "SettingTypeId");
            CreateIndex("DMS.SystemSettings", "ValueTypeId");
        }
    }
}
