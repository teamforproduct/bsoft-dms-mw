namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFieldTypeCodeInProperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Properties", "TypeCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Properties", "TypeCode");
        }
    }
}
