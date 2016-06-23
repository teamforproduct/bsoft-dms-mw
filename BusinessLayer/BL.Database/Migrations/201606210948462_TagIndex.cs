namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TagIndex : DbMigration
    {
        public override void Up()
        {
            DropIndex("DMS.DictionaryTags", new[] { "PositionId" });
            CreateIndex("DMS.DictionaryTags", new[] { "PositionId", "Name", "ClientId" }, unique: true, name: "IX_PositionName");
        }
        
        public override void Down()
        {
            DropIndex("DMS.DictionaryTags", "IX_PositionName");
            CreateIndex("DMS.DictionaryTags", "PositionId");
        }
    }
}
