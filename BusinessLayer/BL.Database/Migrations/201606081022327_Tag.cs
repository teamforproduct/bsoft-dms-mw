namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Tag : DbMigration
    {
        public override void Up()
        {
            AddForeignKey("DMS.AdminSubordinations", "TargetPositionId", "DMS.DictionaryPositions", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.AdminSubordinations", "TargetPositionId", "DMS.DictionaryPositions");
        }
    }
}
