namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModDict : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("DMS.DictionaryAgents", "LanguageId", "DMS.AdminLanguages");
            DropForeignKey("DMS.DictionaryDepartments", "CompanyId", "DMS.DictionaryCompanies");
            DropIndex("DMS.DictionaryCompanies", "IX_Name");
            DropIndex("DMS.DictionaryAgents", new[] { "LanguageId" });
            DropIndex("DMS.DictionaryAgentBanks", new[] { "MFOCode" });
            DropIndex("DMS.DictionaryAgentCompanies", new[] { "FullName" });
            DropIndex("DMS.DictionaryAgentCompanies", new[] { "TaxCode" });
            DropIndex("DMS.DictionaryAgentPersons", new[] { "FullName" });
            DropIndex("DMS.DictionaryAgentContacts", "IX_AgentContactTypeContact");
            DropIndex("DMS.DictionaryAgentEmployees", new[] { "PersonnelNumber" });
            DropIndex("DMS.DictionaryAgentEmployees", new[] { "AgentPersonId" });
            DropPrimaryKey("DMS.DictionaryCompanies");
            CreateTable(
                "DMS.AdminRoleTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 400),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true)
                .Index(t => t.Name, unique: true);
            
            AddColumn("DMS.DictionaryPositions", "Order", c => c.Int(nullable: false));
            AddColumn("DMS.DictionaryCompanies", "FullName", c => c.String(maxLength: 400));
            AddColumn("DMS.DictionaryCompanies", "Description", c => c.String(maxLength: 2000));
            AddColumn("DMS.DictionaryAgentBanks", "ClientId", c => c.Int(nullable: false));
            AddColumn("DMS.DictionaryAgentBanks", "FullName", c => c.String(maxLength: 400));
            AddColumn("DMS.DictionaryAddressTypes", "Code", c => c.String(maxLength: 400));
            AddColumn("DMS.DictionaryAgentCompanies", "ClientId", c => c.Int(nullable: false));
            AddColumn("DMS.DictionaryAgentPersons", "ClientId", c => c.Int(nullable: false));
            AddColumn("DMS.DictionaryAgentContacts", "ClientId", c => c.Int(nullable: false));
            AddColumn("DMS.DictionaryAgentContacts", "IsConfirmed", c => c.Boolean(nullable: false));
            AddColumn("DMS.DictionaryAgentEmployees", "ClientId", c => c.Int(nullable: false));
            AddColumn("DMS.DictionaryAgentUsers", "LanguageId", c => c.Int());
            AddColumn("DMS.AdminRoles", "RoleTypeId", c => c.Int());
            AlterColumn("DMS.DictionaryCompanies", "Id", c => c.Int(nullable: false));
            AlterColumn("DMS.DictionaryAgents", "Name", c => c.String(maxLength: 400));
            AddPrimaryKey("DMS.DictionaryCompanies", "Id");
            CreateIndex("DMS.DictionaryCompanies", "Id");
            CreateIndex("DMS.DictionaryAgents", new[] { "Name", "ClientId" }, unique: true, name: "IX_Name");
            CreateIndex("DMS.DictionaryAgentBanks", "ClientId");
            CreateIndex("DMS.DictionaryAgentBanks", new[] { "FullName", "ClientId" }, unique: true, name: "IX_FullName");
            CreateIndex("DMS.DictionaryAgentBanks", new[] { "MFOCode", "ClientId" }, unique: true, name: "IX_MFOCode");
            CreateIndex("DMS.DictionaryAgentCompanies", "ClientId");
            CreateIndex("DMS.DictionaryAgentCompanies", new[] { "FullName", "ClientId" }, unique: true, name: "IX_FullName");
            CreateIndex("DMS.DictionaryAgentCompanies", new[] { "TaxCode", "ClientId" }, unique: true, name: "IX_TaxCode");
            CreateIndex("DMS.DictionaryAgentPersons", "ClientId");
            CreateIndex("DMS.DictionaryAgentPersons", new[] { "FullName", "ClientId" }, unique: true, name: "IX_FullName");
            CreateIndex("DMS.DictionaryAgentContacts", new[] { "AgentId", "ContactTypeId", "Contact", "ClientId" }, unique: true, name: "IX_AgentContactTypeContact");
            CreateIndex("DMS.DictionaryAgentContacts", "ClientId");
            CreateIndex("DMS.DictionaryAgentEmployees", "ClientId");
            CreateIndex("DMS.DictionaryAgentEmployees", new[] { "PersonnelNumber", "ClientId" }, unique: true, name: "IX_PersonnelNumber");
            CreateIndex("DMS.AdminRoles", "RoleTypeId");
            AddForeignKey("DMS.DictionaryCompanies", "Id", "DMS.DictionaryAgents", "Id");
            AddForeignKey("DMS.AdminRoles", "RoleTypeId", "DMS.AdminRoleTypes", "Id");
            AddForeignKey("DMS.DictionaryDepartments", "CompanyId", "DMS.DictionaryCompanies", "Id");
            DropColumn("DMS.DictionaryCompanies", "Name");
            DropColumn("DMS.DictionaryAgents", "LanguageId");
            DropColumn("DMS.DictionaryAgentEmployees", "AgentPersonId");
        }
        
        public override void Down()
        {
            AddColumn("DMS.DictionaryAgentEmployees", "AgentPersonId", c => c.Int(nullable: false));
            AddColumn("DMS.DictionaryAgents", "LanguageId", c => c.Int());
            AddColumn("DMS.DictionaryCompanies", "Name", c => c.String(maxLength: 400));
            DropForeignKey("DMS.DictionaryDepartments", "CompanyId", "DMS.DictionaryCompanies");
            DropForeignKey("DMS.AdminRoles", "RoleTypeId", "DMS.AdminRoleTypes");
            DropForeignKey("DMS.DictionaryCompanies", "Id", "DMS.DictionaryAgents");
            DropIndex("DMS.AdminRoleTypes", new[] { "Name" });
            DropIndex("DMS.AdminRoleTypes", new[] { "Code" });
            DropIndex("DMS.AdminRoles", new[] { "RoleTypeId" });
            DropIndex("DMS.DictionaryAgentEmployees", "IX_PersonnelNumber");
            DropIndex("DMS.DictionaryAgentEmployees", new[] { "ClientId" });
            DropIndex("DMS.DictionaryAgentContacts", new[] { "ClientId" });
            DropIndex("DMS.DictionaryAgentContacts", "IX_AgentContactTypeContact");
            DropIndex("DMS.DictionaryAgentPersons", "IX_FullName");
            DropIndex("DMS.DictionaryAgentPersons", new[] { "ClientId" });
            DropIndex("DMS.DictionaryAgentCompanies", "IX_TaxCode");
            DropIndex("DMS.DictionaryAgentCompanies", "IX_FullName");
            DropIndex("DMS.DictionaryAgentCompanies", new[] { "ClientId" });
            DropIndex("DMS.DictionaryAgentBanks", "IX_MFOCode");
            DropIndex("DMS.DictionaryAgentBanks", "IX_FullName");
            DropIndex("DMS.DictionaryAgentBanks", new[] { "ClientId" });
            DropIndex("DMS.DictionaryAgents", "IX_Name");
            DropIndex("DMS.DictionaryCompanies", new[] { "Id" });
            DropPrimaryKey("DMS.DictionaryCompanies");
            AlterColumn("DMS.DictionaryAgents", "Name", c => c.String(maxLength: 2000));
            AlterColumn("DMS.DictionaryCompanies", "Id", c => c.Int(nullable: false, identity: true));
            DropColumn("DMS.AdminRoles", "RoleTypeId");
            DropColumn("DMS.DictionaryAgentUsers", "LanguageId");
            DropColumn("DMS.DictionaryAgentEmployees", "ClientId");
            DropColumn("DMS.DictionaryAgentContacts", "IsConfirmed");
            DropColumn("DMS.DictionaryAgentContacts", "ClientId");
            DropColumn("DMS.DictionaryAgentPersons", "ClientId");
            DropColumn("DMS.DictionaryAgentCompanies", "ClientId");
            DropColumn("DMS.DictionaryAddressTypes", "Code");
            DropColumn("DMS.DictionaryAgentBanks", "FullName");
            DropColumn("DMS.DictionaryAgentBanks", "ClientId");
            DropColumn("DMS.DictionaryCompanies", "Description");
            DropColumn("DMS.DictionaryCompanies", "FullName");
            DropColumn("DMS.DictionaryPositions", "Order");
            DropTable("DMS.AdminRoleTypes");
            AddPrimaryKey("DMS.DictionaryCompanies", "Id");
            CreateIndex("DMS.DictionaryAgentEmployees", "AgentPersonId", unique: true);
            CreateIndex("DMS.DictionaryAgentEmployees", "PersonnelNumber", unique: true);
            CreateIndex("DMS.DictionaryAgentContacts", new[] { "AgentId", "ContactTypeId", "Contact" }, unique: true, name: "IX_AgentContactTypeContact");
            CreateIndex("DMS.DictionaryAgentPersons", "FullName", unique: true);
            CreateIndex("DMS.DictionaryAgentCompanies", "TaxCode", unique: true);
            CreateIndex("DMS.DictionaryAgentCompanies", "FullName", unique: true);
            CreateIndex("DMS.DictionaryAgentBanks", "MFOCode", unique: true);
            CreateIndex("DMS.DictionaryAgents", "LanguageId");
            CreateIndex("DMS.DictionaryCompanies", new[] { "Name", "ClientId" }, unique: true, name: "IX_Name");
            AddForeignKey("DMS.DictionaryDepartments", "CompanyId", "DMS.DictionaryCompanies", "Id");
            AddForeignKey("DMS.DictionaryAgents", "LanguageId", "DMS.AdminLanguages", "Id");
        }
    }
}
