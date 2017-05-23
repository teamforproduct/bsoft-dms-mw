namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsContentDeleted : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.DocumentFiles", "IsContentDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("DMS.DocumentFiles", "IsContentDeleted");
        }
    }
}
