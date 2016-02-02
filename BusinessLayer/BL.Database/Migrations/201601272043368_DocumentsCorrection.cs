using System;
using System.Data.Entity.Migrations;

namespace BL.Database.Migrations
{
   
    public partial class DocumentsCorrection : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.DictionaryDepartments", new[] { "ChiefPositionId" });
            AlterColumn("dbo.DictionaryDepartments", "ChiefPositionId", c => c.Int());
            CreateIndex("dbo.DictionaryDepartments", "ChiefPositionId");
            DropColumn("dbo.Documents", "DocumentDirectionId");
            DropColumn("dbo.Documents", "DocumentTypeId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Documents", "DocumentTypeId", c => c.Int(nullable: false));
            AddColumn("dbo.Documents", "DocumentDirectionId", c => c.Int(nullable: false));
            DropIndex("dbo.DictionaryDepartments", new[] { "ChiefPositionId" });
            AlterColumn("dbo.DictionaryDepartments", "ChiefPositionId", c => c.Int(nullable: false));
            CreateIndex("dbo.DictionaryDepartments", "ChiefPositionId");
        }
    }
}
