namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DocSavFilters : DbMigration
    {
        public override void Up()
        {
            DropIndex("DMS.DocumentSavedFilters", "IX_IconPosition");
            AddColumn("DMS.DocumentSavedFilters", "Name", c => c.String(maxLength: 400));
            CreateIndex("DMS.DocumentSavedFilters", new[] { "Name", "PositionId", "ClientId" }, unique: true, name: "IX_NamePosition");
        }
        
        public override void Down()
        {
            DropIndex("DMS.DocumentSavedFilters", "IX_NamePosition");
            DropColumn("DMS.DocumentSavedFilters", "Name");
            CreateIndex("DMS.DocumentSavedFilters", new[] { "Icon", "PositionId", "ClientId" }, unique: true, name: "IX_IconPosition");
        }
    }
}
