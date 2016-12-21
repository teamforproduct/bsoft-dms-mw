namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgentPeople : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "DMS.DictionaryAgentPeoples",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        FullName = c.String(maxLength: 400),
                        LastName = c.String(maxLength: 2000),
                        FirstName = c.String(maxLength: 2000),
                        MiddleName = c.String(maxLength: 2000),
                        BirthDate = c.DateTime(),
                        IsMale = c.Boolean(nullable: false),
                        PassportSerial = c.String(maxLength: 2000),
                        PassportNumber = c.Int(),
                        PassportText = c.String(maxLength: 2000),
                        PassportDate = c.DateTime(),
                        TaxCode = c.String(maxLength: 2000),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.Id)
                .Index(t => t.Id)
                .Index(t => t.ClientId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.DictionaryAgentPeoples", "Id", "DMS.DictionaryAgents");
            DropIndex("DMS.DictionaryAgentPeoples", new[] { "ClientId" });
            DropIndex("DMS.DictionaryAgentPeoples", new[] { "Id" });
            DropTable("DMS.DictionaryAgentPeoples");
        }
    }
}
