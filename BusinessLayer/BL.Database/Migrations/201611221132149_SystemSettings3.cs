namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SystemSettings3 : DbMigration
    {
        public override void Up()
        {
            AddForeignKey("DMS.SystemSettings", "SettingTypeId", "DMS.DictionarySettingTypes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.SystemSettings", "SettingTypeId", "DMS.DictionarySettingTypes");
        }
    }
}
