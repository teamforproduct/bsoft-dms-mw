namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmployeeDepartments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "DMS.AdminEmployeeDepartments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        DepartmentId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryDepartments", t => t.DepartmentId)
                .ForeignKey("DMS.DictionaryAgentEmployees", t => t.EmployeeId)
                .Index(t => new { t.EmployeeId, t.DepartmentId }, unique: true, name: "IX_EmployeeDepartment")
                .Index(t => t.EmployeeId)
                .Index(t => t.DepartmentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.AdminEmployeeDepartments", "EmployeeId", "DMS.DictionaryAgentEmployees");
            DropForeignKey("DMS.AdminEmployeeDepartments", "DepartmentId", "DMS.DictionaryDepartments");
            DropIndex("DMS.AdminEmployeeDepartments", new[] { "DepartmentId" });
            DropIndex("DMS.AdminEmployeeDepartments", new[] { "EmployeeId" });
            DropIndex("DMS.AdminEmployeeDepartments", "IX_EmployeeDepartment");
            DropTable("DMS.AdminEmployeeDepartments");
        }
    }
}
