namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsReg : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Documents", "IsRegistered", c => c.Boolean(nullable: false));
            AddColumn("dbo.DocumentFiles", "Extension", c => c.String());
            DropColumn("dbo.DocumentFiles", "Extention");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DocumentFiles", "Extention", c => c.String());
            DropColumn("dbo.DocumentFiles", "Extension");
            DropColumn("dbo.Documents", "IsRegistered");
        }
    }
}
