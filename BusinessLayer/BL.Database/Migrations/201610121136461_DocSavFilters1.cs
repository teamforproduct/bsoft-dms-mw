namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DocSavFilters1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("DMS.DocumentSavedFilters", "PositionId", "DMS.DictionaryPositions");
            DropIndex("DMS.DocumentSavedFilters", "IX_NamePosition");
            DropIndex("DMS.DocumentSavedFilters", new[] { "PositionId" });
            AddColumn("DMS.DocumentSavedFilters", "UserId", c => c.Int());
            CreateIndex("DMS.DocumentSavedFilters", new[] { "Name", "UserId", "ClientId" }, unique: true, name: "IX_NameUser");
            CreateIndex("DMS.DocumentSavedFilters", "UserId");
            AddForeignKey("DMS.DocumentSavedFilters", "UserId", "DMS.DictionaryAgentUsers", "Id");
            DropColumn("DMS.DocumentSavedFilters", "PositionId");
        }
        
        public override void Down()
        {
            AddColumn("DMS.DocumentSavedFilters", "PositionId", c => c.Int());
            DropForeignKey("DMS.DocumentSavedFilters", "UserId", "DMS.DictionaryAgentUsers");
            DropIndex("DMS.DocumentSavedFilters", new[] { "UserId" });
            DropIndex("DMS.DocumentSavedFilters", "IX_NameUser");
            DropColumn("DMS.DocumentSavedFilters", "UserId");
            CreateIndex("DMS.DocumentSavedFilters", "PositionId");
            CreateIndex("DMS.DocumentSavedFilters", new[] { "Name", "PositionId", "ClientId" }, unique: true, name: "IX_NamePosition");
            AddForeignKey("DMS.DocumentSavedFilters", "PositionId", "DMS.DictionaryPositions", "Id");
        }
    }
}
