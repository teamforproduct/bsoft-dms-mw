namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsReg : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Documents", "IsRegistered", c => c.Boolean(nullable: false));
            AddColumn("dbo.Files", "Extension", c => c.String());
            DropColumn("dbo.Files", "Extention");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Files", "Extention", c => c.String());
            DropColumn("dbo.Files", "Extension");
            DropColumn("dbo.Documents", "IsRegistered");
        }
    }
}
