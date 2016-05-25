namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFieldSelectTableToProperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.Properties", "SelectTable", c => c.String(maxLength: 2000));
        }
        
        public override void Down()
        {
            DropColumn("DMS.Properties", "SelectTable");
        }
    }
}
