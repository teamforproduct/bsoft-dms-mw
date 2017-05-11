namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Departments : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "DMS.DictionaryDepartments", name: "Code", newName: "Index");
        }
        
        public override void Down()
        {
            RenameColumn(table: "DMS.DictionaryDepartments", name: "Index", newName: "Code");
        }
    }
}
