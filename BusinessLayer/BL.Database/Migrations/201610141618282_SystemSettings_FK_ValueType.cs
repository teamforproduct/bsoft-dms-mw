namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SystemSettings_FK_ValueType : DbMigration
    {
        public override void Up()
        {
            CreateIndex("DMS.SystemSettings", "ValueType");
            AddForeignKey("DMS.SystemSettings", "ValueType", "DMS.SystemValueTypes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.SystemSettings", "ValueType", "DMS.SystemValueTypes");
            DropIndex("DMS.SystemSettings", new[] { "ValueType" });
        }
    }
}
