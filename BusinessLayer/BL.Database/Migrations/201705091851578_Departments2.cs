namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Departments2 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "DMS.DictionaryDepartments", name: "FullPath", newName: "Code");
        }
        
        public override void Down()
        {
            RenameColumn(table: "DMS.DictionaryDepartments", name: "Code", newName: "FullPath");
        }
    }
}
