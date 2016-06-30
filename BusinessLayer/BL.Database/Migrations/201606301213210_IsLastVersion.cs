namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsLastVersion : DbMigration
    {
        public override void Up()
        {
            DropColumn("DMS.DocumentFiles", "IsLastVersion");
        }
        
        public override void Down()
        {
            AddColumn("DMS.DocumentFiles", "IsLastVersion", c => c.Boolean(nullable: false));
        }
    }
}
