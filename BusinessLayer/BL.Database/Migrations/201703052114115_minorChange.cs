namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class minorChange : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.TemplateDocuments", "EntityTypeId", c => c.Int(nullable: false));
            CreateIndex("DMS.TemplateDocuments", "EntityTypeId");
        }
        
        public override void Down()
        {
            DropIndex("DMS.TemplateDocuments", new[] { "EntityTypeId" });
            DropColumn("DMS.TemplateDocuments", "EntityTypeId");
        }
    }
}
