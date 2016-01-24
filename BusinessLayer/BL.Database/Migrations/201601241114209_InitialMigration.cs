namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DictionaryAgents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        TaxCode = c.String(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DictionaryPositions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParentId = c.Int(),
                        Name = c.String(),
                        DepartmentId = c.Int(nullable: false),
                        ExecutorAgentId = c.Int(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                        ParentPosition_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DictionaryPositions", t => t.ParentPosition_Id)
                .ForeignKey("dbo.DictionaryDepartments", t => t.DepartmentId, cascadeDelete: true)
                .ForeignKey("dbo.DictionaryAgents", t => t.ExecutorAgentId)
                .Index(t => t.DepartmentId)
                .Index(t => t.ExecutorAgentId)
                .Index(t => t.ParentPosition_Id);
            
            CreateTable(
                "dbo.DictionaryDepartments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParentId = c.Int(),
                        CompanyId = c.Int(nullable: false),
                        Name = c.String(),
                        ChiefPositionId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                        ParentDepartment_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DictionaryDepartments", t => t.ParentDepartment_Id)
                .ForeignKey("dbo.DictionaryCompanies", t => t.CompanyId, cascadeDelete: true)
                .Index(t => t.CompanyId)
                .Index(t => t.ParentDepartment_Id);
            
            CreateTable(
                "dbo.DictionaryCompanies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DictionaryPositions", "ExecutorAgentId", "dbo.DictionaryAgents");
            DropForeignKey("dbo.DictionaryPositions", "DepartmentId", "dbo.DictionaryDepartments");
            DropForeignKey("dbo.DictionaryDepartments", "CompanyId", "dbo.DictionaryCompanies");
            DropForeignKey("dbo.DictionaryDepartments", "ParentDepartment_Id", "dbo.DictionaryDepartments");
            DropForeignKey("dbo.DictionaryPositions", "ParentPosition_Id", "dbo.DictionaryPositions");
            DropIndex("dbo.DictionaryDepartments", new[] { "ParentDepartment_Id" });
            DropIndex("dbo.DictionaryDepartments", new[] { "CompanyId" });
            DropIndex("dbo.DictionaryPositions", new[] { "ParentPosition_Id" });
            DropIndex("dbo.DictionaryPositions", new[] { "ExecutorAgentId" });
            DropIndex("dbo.DictionaryPositions", new[] { "DepartmentId" });
            DropTable("dbo.DictionaryCompanies");
            DropTable("dbo.DictionaryDepartments");
            DropTable("dbo.DictionaryPositions");
            DropTable("dbo.DictionaryAgents");
        }
    }
}
