namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tempreg : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DocumentTemporaryRegistrations",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        RegistrationJournalId = c.Int(nullable: false),
                        RegistrationNumber = c.Int(nullable: false),
                        RegistrationNumberSuffix = c.String(),
                        RegistrationNumberPrefix = c.String(),
                        RegistrationDate = c.DateTime(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DictionaryRegistrationJournals", t => t.RegistrationJournalId)
                .ForeignKey("dbo.Documents", t => t.Id)
                .Index(t => t.Id)
                .Index(t => t.RegistrationJournalId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DocumentTemporaryRegistrations", "Id", "dbo.Documents");
            DropForeignKey("dbo.DocumentTemporaryRegistrations", "RegistrationJournalId", "dbo.DictionaryRegistrationJournals");
            DropIndex("dbo.DocumentTemporaryRegistrations", new[] { "RegistrationJournalId" });
            DropIndex("dbo.DocumentTemporaryRegistrations", new[] { "Id" });
            DropTable("dbo.DocumentTemporaryRegistrations");
        }
    }
}
