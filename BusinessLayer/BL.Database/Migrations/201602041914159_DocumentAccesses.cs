namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DocumentAccesses : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DocumentAccesses", "IsFavourite", c => c.Boolean(nullable: false));
            DropColumn("dbo.DocumentAccesses", "IsFavourtite");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DocumentAccesses", "IsFavourtite", c => c.Boolean(nullable: false));
            DropColumn("dbo.DocumentAccesses", "IsFavourite");
        }
    }
}
