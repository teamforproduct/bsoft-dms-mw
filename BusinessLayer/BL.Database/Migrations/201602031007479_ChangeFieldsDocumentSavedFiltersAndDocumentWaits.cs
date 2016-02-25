namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeFieldsDocumentSavedFiltersAndDocumentWaits : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Waits", "Description", c => c.String());
            AddColumn("dbo.Waits", "DueDate", c => c.DateTime());
            AddColumn("dbo.Waits", "AttentionDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Waits", "AttentionDate");
            DropColumn("dbo.Waits", "DueDate");
            DropColumn("dbo.Waits", "Description");
        }
    }
}
