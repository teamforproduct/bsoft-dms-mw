namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class agent : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DictionaryAgentAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AgentId = c.Int(nullable: false),
                        Name = c.String(),
                        AgentBankId = c.Int(nullable: false),
                        AccountNumber = c.String(),
                        IsMain = c.Boolean(nullable: false),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DictionaryAgents", t => t.AgentId)
                .ForeignKey("dbo.DictionaryAgentBanks", t => t.AgentBankId)
                .Index(t => t.AgentId)
                .Index(t => t.AgentBankId);
            
            CreateTable(
                "dbo.DictionaryAgentBanks",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        MFOCode = c.String(),
                        Swift = c.String(),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DictionaryAgents", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.DictionaryAgentAddresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AgentId = c.Int(nullable: false),
                        AdressTypeId = c.Int(nullable: false),
                        PostCode = c.String(),
                        Address = c.String(),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DictionaryAddressTypes", t => t.AdressTypeId)
                .ForeignKey("dbo.DictionaryAgents", t => t.AgentId)
                .Index(t => t.AgentId)
                .Index(t => t.AdressTypeId);
            
            CreateTable(
                "dbo.DictionaryAddressTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DictionaryAgentCompanies",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        FullName = c.String(),
                        TaxCode = c.String(),
                        OKPOCode = c.String(),
                        VATCode = c.String(),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DictionaryAgents", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.DictionaryAgentEmployees",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        PersonnelNumber = c.String(),
                        AgentPersonId = c.Int(nullable: false),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DictionaryAgents", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.DictionaryAgentPhoneNumbers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AgentId = c.Int(nullable: false),
                        PhoneTypeId = c.Int(nullable: false),
                        PhoneNumber = c.String(),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DictionaryAgents", t => t.AgentId)
                .ForeignKey("dbo.DictionaryPhoneNumberTypes", t => t.PhoneTypeId)
                .Index(t => t.AgentId)
                .Index(t => t.PhoneTypeId);
            
            CreateTable(
                "dbo.DictionaryPhoneNumberTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DictionaryResidentTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.DictionaryAgents", "ResidentTypeId", c => c.Int());
            AddColumn("dbo.DictionaryAgents", "IsBank", c => c.Boolean(nullable: false));
            AddColumn("dbo.DictionaryAgents", "Description", c => c.String());
            AddColumn("dbo.DictionaryAgents", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.DictionaryAgentPersons", "FullName", c => c.String());
            AddColumn("dbo.DictionaryAgentPersons", "LastName", c => c.String());
            AddColumn("dbo.DictionaryAgentPersons", "FirstName", c => c.String());
            AddColumn("dbo.DictionaryAgentPersons", "MiddleName", c => c.String());
            AddColumn("dbo.DictionaryAgentPersons", "TaxCode", c => c.String());
            AddColumn("dbo.DictionaryAgentPersons", "IsMale", c => c.Boolean(nullable: false));
            AddColumn("dbo.DictionaryAgentPersons", "PassportSerial", c => c.String());
            AddColumn("dbo.DictionaryAgentPersons", "PassportNumber", c => c.Int());
            AddColumn("dbo.DictionaryAgentPersons", "PassportText", c => c.String());
            AddColumn("dbo.DictionaryAgentPersons", "PassportDate", c => c.DateTime());
            AddColumn("dbo.DictionaryAgentPersons", "BirthDate", c => c.DateTime());
            AddColumn("dbo.DictionaryAgentPersons", "AgentCompanyId", c => c.Int());
            AddColumn("dbo.DictionaryAgentPersons", "Description", c => c.String());
            AddColumn("dbo.DictionaryAgentPersons", "IsActive", c => c.Boolean(nullable: false));
            CreateIndex("dbo.DictionaryAgents", "ResidentTypeId");
            CreateIndex("dbo.DictionaryAgentPersons", "AgentCompanyId");
            CreateIndex("dbo.Documents", "SenderAgentPersonId");
            CreateIndex("dbo.TemplateDocuments", "SenderAgentPersonId");
            AddForeignKey("dbo.DictionaryAgentPersons", "AgentCompanyId", "dbo.DictionaryAgentCompanies", "Id");
            AddForeignKey("dbo.DictionaryAgents", "ResidentTypeId", "dbo.DictionaryResidentTypes", "Id");
            AddForeignKey("dbo.Documents", "SenderAgentPersonId", "dbo.DictionaryAgentPersons", "Id");
            AddForeignKey("dbo.TemplateDocuments", "SenderAgentPersonId", "dbo.DictionaryAgentPersons", "Id");
            DropColumn("dbo.DictionaryAgentPersons", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DictionaryAgentPersons", "Name", c => c.String());
            DropForeignKey("dbo.TemplateDocuments", "SenderAgentPersonId", "dbo.DictionaryAgentPersons");
            DropForeignKey("dbo.Documents", "SenderAgentPersonId", "dbo.DictionaryAgentPersons");
            DropForeignKey("dbo.DictionaryAgents", "ResidentTypeId", "dbo.DictionaryResidentTypes");
            DropForeignKey("dbo.DictionaryAgentPhoneNumbers", "PhoneTypeId", "dbo.DictionaryPhoneNumberTypes");
            DropForeignKey("dbo.DictionaryAgentPhoneNumbers", "AgentId", "dbo.DictionaryAgents");
            DropForeignKey("dbo.DictionaryAgentEmployees", "Id", "dbo.DictionaryAgents");
            DropForeignKey("dbo.DictionaryAgentCompanies", "Id", "dbo.DictionaryAgents");
            DropForeignKey("dbo.DictionaryAgentPersons", "AgentCompanyId", "dbo.DictionaryAgentCompanies");
            DropForeignKey("dbo.DictionaryAgentBanks", "Id", "dbo.DictionaryAgents");
            DropForeignKey("dbo.DictionaryAgentAddresses", "AgentId", "dbo.DictionaryAgents");
            DropForeignKey("dbo.DictionaryAgentAddresses", "AdressTypeId", "dbo.DictionaryAddressTypes");
            DropForeignKey("dbo.DictionaryAgentAccounts", "AgentBankId", "dbo.DictionaryAgentBanks");
            DropForeignKey("dbo.DictionaryAgentAccounts", "AgentId", "dbo.DictionaryAgents");
            DropIndex("dbo.TemplateDocuments", new[] { "SenderAgentPersonId" });
            DropIndex("dbo.Documents", new[] { "SenderAgentPersonId" });
            DropIndex("dbo.DictionaryAgentPhoneNumbers", new[] { "PhoneTypeId" });
            DropIndex("dbo.DictionaryAgentPhoneNumbers", new[] { "AgentId" });
            DropIndex("dbo.DictionaryAgentEmployees", new[] { "Id" });
            DropIndex("dbo.DictionaryAgentPersons", new[] { "AgentCompanyId" });
            DropIndex("dbo.DictionaryAgentCompanies", new[] { "Id" });
            DropIndex("dbo.DictionaryAgentAddresses", new[] { "AdressTypeId" });
            DropIndex("dbo.DictionaryAgentAddresses", new[] { "AgentId" });
            DropIndex("dbo.DictionaryAgentBanks", new[] { "Id" });
            DropIndex("dbo.DictionaryAgentAccounts", new[] { "AgentBankId" });
            DropIndex("dbo.DictionaryAgentAccounts", new[] { "AgentId" });
            DropIndex("dbo.DictionaryAgents", new[] { "ResidentTypeId" });
            DropColumn("dbo.DictionaryAgentPersons", "IsActive");
            DropColumn("dbo.DictionaryAgentPersons", "Description");
            DropColumn("dbo.DictionaryAgentPersons", "AgentCompanyId");
            DropColumn("dbo.DictionaryAgentPersons", "BirthDate");
            DropColumn("dbo.DictionaryAgentPersons", "PassportDate");
            DropColumn("dbo.DictionaryAgentPersons", "PassportText");
            DropColumn("dbo.DictionaryAgentPersons", "PassportNumber");
            DropColumn("dbo.DictionaryAgentPersons", "PassportSerial");
            DropColumn("dbo.DictionaryAgentPersons", "IsMale");
            DropColumn("dbo.DictionaryAgentPersons", "TaxCode");
            DropColumn("dbo.DictionaryAgentPersons", "MiddleName");
            DropColumn("dbo.DictionaryAgentPersons", "FirstName");
            DropColumn("dbo.DictionaryAgentPersons", "LastName");
            DropColumn("dbo.DictionaryAgentPersons", "FullName");
            DropColumn("dbo.DictionaryAgents", "IsActive");
            DropColumn("dbo.DictionaryAgents", "Description");
            DropColumn("dbo.DictionaryAgents", "IsBank");
            DropColumn("dbo.DictionaryAgents", "ResidentTypeId");
            DropTable("dbo.DictionaryResidentTypes");
            DropTable("dbo.DictionaryPhoneNumberTypes");
            DropTable("dbo.DictionaryAgentPhoneNumbers");
            DropTable("dbo.DictionaryAgentEmployees");
            DropTable("dbo.DictionaryAgentCompanies");
            DropTable("dbo.DictionaryAddressTypes");
            DropTable("dbo.DictionaryAgentAddresses");
            DropTable("dbo.DictionaryAgentBanks");
            DropTable("dbo.DictionaryAgentAccounts");
        }
    }
}
