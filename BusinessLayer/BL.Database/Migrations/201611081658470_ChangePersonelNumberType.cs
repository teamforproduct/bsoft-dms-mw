namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangePersonelNumberType : DbMigration
    {
        public override void Up()
        {
            DropIndex("DMS.DictionaryAgentEmployees", "IX_PersonnelNumber");
            AlterColumn("DMS.DictionaryAgentEmployees", "PersonnelNumber", c => c.Int(nullable: false));
            CreateIndex("DMS.DictionaryAgentEmployees", new[] { "PersonnelNumber", "ClientId" }, unique: true, name: "IX_PersonnelNumber");
        }
        
        public override void Down()
        {
            DropIndex("DMS.DictionaryAgentEmployees", "IX_PersonnelNumber");
            AlterColumn("DMS.DictionaryAgentEmployees", "PersonnelNumber", c => c.String(maxLength: 400));
            CreateIndex("DMS.DictionaryAgentEmployees", new[] { "PersonnelNumber", "ClientId" }, unique: true, name: "IX_PersonnelNumber");
        }
    }
}
