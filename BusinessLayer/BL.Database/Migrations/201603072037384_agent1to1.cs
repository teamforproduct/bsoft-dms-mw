namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class agent1to1 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.DictionaryAgentPersons");
            AlterColumn("dbo.DictionaryAgentPersons", "Id", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.DictionaryAgentPersons", "Id");
            CreateIndex("dbo.DictionaryAgentPersons", "Id");
            AddForeignKey("dbo.DictionaryAgentPersons", "Id", "dbo.DictionaryAgents", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DictionaryAgentPersons", "Id", "dbo.DictionaryAgents");
            DropIndex("dbo.DictionaryAgentPersons", new[] { "Id" });
            DropPrimaryKey("dbo.DictionaryAgentPersons");
            AlterColumn("dbo.DictionaryAgentPersons", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.DictionaryAgentPersons", "Id");
        }
    }
}
