namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModuleFeature_Columns : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.SystemActions", "Module", c => c.String(maxLength: 2000));
            AddColumn("DMS.SystemActions", "Feature", c => c.String(maxLength: 2000));
            AddColumn("DMS.SystemActions", "CRUR", c => c.String(maxLength: 2000));
        }
        
        public override void Down()
        {
            DropColumn("DMS.SystemActions", "CRUR");
            DropColumn("DMS.SystemActions", "Feature");
            DropColumn("DMS.SystemActions", "Module");
        }
    }
}
