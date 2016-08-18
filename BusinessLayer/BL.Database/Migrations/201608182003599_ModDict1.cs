namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModDict1 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("DMS.DictionaryCompanies", new[] { "FullName", "ClientId" }, unique: true, name: "IX_FullName");
            CreateIndex("DMS.DictionaryAddressTypes", new[] { "Code", "ClientId" }, unique: true, name: "IX_Code");
        }
        
        public override void Down()
        {
            DropIndex("DMS.DictionaryAddressTypes", "IX_Code");
            DropIndex("DMS.DictionaryCompanies", "IX_FullName");
        }
    }
}
