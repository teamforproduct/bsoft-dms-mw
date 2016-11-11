namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OffUniqueIndexForAgents : DbMigration
    {
        public override void Up()
        {
            DropIndex("DMS.DictionaryAgents", "IX_Name");
            DropIndex("DMS.DictionaryAgentPersons", "IX_FullName");
        }
        
        public override void Down()
        {
            CreateIndex("DMS.DictionaryAgentPersons", new[] { "FullName", "ClientId" }, unique: true, name: "IX_FullName");
            CreateIndex("DMS.DictionaryAgents", new[] { "Name", "ClientId" }, unique: true, name: "IX_Name");
        }
    }
}
