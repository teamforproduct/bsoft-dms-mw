namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Sub : DbMigration
    {
        public override void Up()
        {
            AddForeignKey("DMS.AdminSubordinations", "SourcePositionId", "DMS.DictionaryPositions", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.AdminSubordinations", "SourcePositionId", "DMS.DictionaryPositions");
        }
    }
}
