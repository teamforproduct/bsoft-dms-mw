namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class department : DbMigration
    {
        public override void Up()
        {
            AddForeignKey("DMS.DictionaryPositions", "DepartmentId", "DMS.DictionaryDepartments", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.DictionaryPositions", "DepartmentId", "DMS.DictionaryDepartments");
        }
    }
}
