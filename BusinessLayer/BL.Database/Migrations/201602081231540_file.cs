namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class file : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Files", "FileType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Files", "FileType");
        }
    }
}
