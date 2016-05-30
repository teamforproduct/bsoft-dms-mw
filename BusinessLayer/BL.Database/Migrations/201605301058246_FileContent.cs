namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FileContent : DbMigration
    {
        public override void Up()
        {
            AlterColumn("DMS.DocumentFiles", "Content", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("DMS.DocumentFiles", "Content", c => c.Binary());
        }
    }
}
