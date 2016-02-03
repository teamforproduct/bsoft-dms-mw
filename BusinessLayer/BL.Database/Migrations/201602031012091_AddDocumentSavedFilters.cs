namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDocumentSavedFilters : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DocumentSavedFilters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PositionId = c.Int(),
                        Icon = c.String(),
                        Filter = c.String(),
                        IsCommon = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DictionaryPositions", t => t.PositionId)
                .Index(t => t.PositionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DocumentSavedFilters", "PositionId", "dbo.DictionaryPositions");
            DropIndex("dbo.DocumentSavedFilters", new[] { "PositionId" });
            DropTable("dbo.DocumentSavedFilters");
        }
    }
}
