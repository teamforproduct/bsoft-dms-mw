namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Files : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.DocumentFiles", "IsMainVersion", c => c.Boolean(nullable: false));
            AddColumn("DMS.DocumentFiles", "IsLastVersion", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("DMS.DocumentFiles", "IsLastVersion");
            DropColumn("DMS.DocumentFiles", "IsMainVersion");
        }
    }
}
