namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Departments3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.DictionaryDepartments", "Path", c => c.String(maxLength: 2000));
        }
        
        public override void Down()
        {
            DropColumn("DMS.DictionaryDepartments", "Path");
        }
    }
}
