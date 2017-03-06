namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PositionExecutorTypes_Description : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.DicPositionExecutorTypes", "Description", c => c.String(maxLength: 400));
        }
        
        public override void Down()
        {
            DropColumn("DMS.DicPositionExecutorTypes", "Description");
        }
    }
}
