namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Code1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AdminAccessLevels", "Code", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AdminAccessLevels", "Code");
        }
    }
}
