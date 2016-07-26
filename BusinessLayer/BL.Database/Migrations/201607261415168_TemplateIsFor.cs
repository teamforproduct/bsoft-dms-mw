namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TemplateIsFor : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.TemplateDocuments", "IsForProject", c => c.Boolean(nullable: false));
            AddColumn("DMS.TemplateDocuments", "IsForDocument", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("DMS.TemplateDocuments", "IsForDocument");
            DropColumn("DMS.TemplateDocuments", "IsForProject");
        }
    }
}
