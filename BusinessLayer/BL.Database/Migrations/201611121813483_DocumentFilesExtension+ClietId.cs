namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DocumentFilesExtensionClietId : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.DocumentFiles", "ClientId", c => c.Int(nullable: false));
            CreateIndex("DMS.DocumentFiles", "ClientId");
        }
        
        public override void Down()
        {
            DropIndex("DMS.DocumentFiles", new[] { "ClientId" });
            DropColumn("DMS.DocumentFiles", "ClientId");
        }
    }
}
