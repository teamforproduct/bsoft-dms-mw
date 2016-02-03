namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeFieldsDocumentSavedFiltersAndDocumentWaits : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DocumentWaits", "Description", c => c.String());
            AddColumn("dbo.DocumentWaits", "DueDate", c => c.DateTime());
            AddColumn("dbo.DocumentWaits", "AttentionDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DocumentWaits", "AttentionDate");
            DropColumn("dbo.DocumentWaits", "DueDate");
            DropColumn("dbo.DocumentWaits", "Description");
        }
    }
}
