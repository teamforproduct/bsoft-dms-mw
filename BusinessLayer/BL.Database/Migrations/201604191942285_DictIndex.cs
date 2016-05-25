namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DictIndex : DbMigration
    {
        public override void Up()
        {
            DropIndex("DMS.DictionaryDepartments", new[] { "ParentId" });
            DropIndex("DMS.DictionaryDepartments", new[] { "CompanyId" });
            DropIndex("DMS.DictionaryAgentAccounts", new[] { "AgentId" });
            DropIndex("DMS.DictionaryAgentAddresses", new[] { "AgentId" });
            DropIndex("DMS.DictionaryAgentAddresses", new[] { "AdressTypeId" });
            DropIndex("DMS.DictionaryAgentContacts", new[] { "AgentId" });
            DropIndex("DMS.DictionaryAgentContacts", new[] { "ContactTypeId" });
            DropIndex("DMS.DictionaryPositionExecutors", new[] { "AgentId" });
            DropIndex("DMS.DictionaryPositionExecutors", new[] { "PositionId" });
            DropIndex("DMS.DictionaryStandartSendLists", new[] { "PositionId" });
            DropIndex("DMS.DictionaryTags", new[] { "PositionId" });
            DropIndex("DMS.CustomDictionaries", new[] { "DictionaryTypeId" });
            CreateIndex("DMS.DictionaryDepartments", new[] { "CompanyId", "ParentId", "Name" }, name: "IX_CompanyParentName");
            CreateIndex("DMS.DictionaryDepartments", "ParentId");
            CreateIndex("DMS.DictionaryCompanies", "Name", unique: true);
            CreateIndex("DMS.DictionaryAgents", "Name", unique: true);
            CreateIndex("DMS.DictionaryAgentAccounts", new[] { "AgentId", "Name" }, unique: true, name: "IX_AgentName");
            CreateIndex("DMS.DictionaryAgentBanks", "MFOCode", unique: true);
            CreateIndex("DMS.DictionaryAgentAddresses", new[] { "AgentId", "AdressTypeId" }, unique: true, name: "IX_AdressType");
            CreateIndex("DMS.DictionaryAgentAddresses", "AdressTypeId");
            CreateIndex("DMS.DictionaryAddressTypes", "Name", unique: true);
            CreateIndex("DMS.DictionaryAgentCompanies", "FullName", unique: true);
            CreateIndex("DMS.DictionaryAgentCompanies", "TaxCode", unique: true);
            CreateIndex("DMS.DictionaryAgentPersons", "FullName", unique: true);
            CreateIndex("DMS.DictionaryAgentContacts", new[] { "AgentId", "ContactTypeId", "Contact" }, unique: true, name: "IX_AgentContactTypeContact");
            CreateIndex("DMS.DictionaryAgentContacts", "ContactTypeId");
            CreateIndex("DMS.DictionaryContactTypes", "Code", unique: true);
            CreateIndex("DMS.DictionaryContactTypes", "Name", unique: true);
            CreateIndex("DMS.DictionaryAgentEmployees", "PersonnelNumber", unique: true);
            CreateIndex("DMS.DictionaryAgentEmployees", "AgentPersonId", unique: true);
            CreateIndex("DMS.DictionaryAgentUsers", "UserId", unique: true);
            CreateIndex("DMS.DictionaryResidentTypes", "Name", unique: true);
            CreateIndex("DMS.DictionaryPositionExecutors", "AgentId");
            CreateIndex("DMS.DictionaryPositionExecutors", new[] { "PositionId", "AgentId", "StartDate" }, unique: true, name: "IX_PositionAgentStartDate");
            CreateIndex("DMS.DicPositionExecutorTypes", "Code", unique: true);
            CreateIndex("DMS.DicPositionExecutorTypes", "Name", unique: true);
            CreateIndex("DMS.DictionaryStandartSendLists", new[] { "PositionId", "Name" }, unique: true, name: "IX_PositionName");
            CreateIndex("DMS.DictionarySendTypes", "Name", unique: true);
            CreateIndex("DMS.DictionarySubordinationTypes", "Code", unique: true);
            CreateIndex("DMS.DictionarySubordinationTypes", "Name", unique: true);
            CreateIndex("DMS.DictionaryTags", new[] { "PositionId", "Name" }, unique: true, name: "IX_PositionName");
            CreateIndex("DMS.DictionaryDocumentSubjects", "Name", unique: true);
            CreateIndex("DMS.DictionarySubscriptionStates", "Code", unique: true);
            CreateIndex("DMS.DictionarySubscriptionStates", "Name", unique: true);
            CreateIndex("DMS.DictionaryEventTypes", "Name", unique: true);
            CreateIndex("DMS.DictionaryImportanceEventTypes", "Name", unique: true);
            CreateIndex("DMS.DictionaryResultTypes", "Name", unique: true);
            CreateIndex("DMS.DictionaryLinkTypes", "Name", unique: true);
            CreateIndex("DMS.DictionaryDocumentDirections", "Code", unique: true);
            CreateIndex("DMS.DictionaryDocumentDirections", "Name", unique: true);
            CreateIndex("DMS.DictionaryDocumentTypes", "Name", unique: true);
            CreateIndex("DMS.CustomDictionaries", new[] { "DictionaryTypeId", "Code" }, unique: true, name: "IX_DictionaryTypeCode");
            CreateIndex("DMS.CustomDictionaryTypes", "Code", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("DMS.CustomDictionaryTypes", new[] { "Code" });
            DropIndex("DMS.CustomDictionaries", "IX_DictionaryTypeCode");
            DropIndex("DMS.DictionaryDocumentTypes", new[] { "Name" });
            DropIndex("DMS.DictionaryDocumentDirections", new[] { "Name" });
            DropIndex("DMS.DictionaryDocumentDirections", new[] { "Code" });
            DropIndex("DMS.DictionaryLinkTypes", new[] { "Name" });
            DropIndex("DMS.DictionaryResultTypes", new[] { "Name" });
            DropIndex("DMS.DictionaryImportanceEventTypes", new[] { "Name" });
            DropIndex("DMS.DictionaryEventTypes", new[] { "Name" });
            DropIndex("DMS.DictionarySubscriptionStates", new[] { "Name" });
            DropIndex("DMS.DictionarySubscriptionStates", new[] { "Code" });
            DropIndex("DMS.DictionaryDocumentSubjects", new[] { "Name" });
            DropIndex("DMS.DictionaryTags", "IX_PositionName");
            DropIndex("DMS.DictionarySubordinationTypes", new[] { "Name" });
            DropIndex("DMS.DictionarySubordinationTypes", new[] { "Code" });
            DropIndex("DMS.DictionarySendTypes", new[] { "Name" });
            DropIndex("DMS.DictionaryStandartSendLists", "IX_PositionName");
            DropIndex("DMS.DicPositionExecutorTypes", new[] { "Name" });
            DropIndex("DMS.DicPositionExecutorTypes", new[] { "Code" });
            DropIndex("DMS.DictionaryPositionExecutors", "IX_PositionAgentStartDate");
            DropIndex("DMS.DictionaryPositionExecutors", new[] { "AgentId" });
            DropIndex("DMS.DictionaryResidentTypes", new[] { "Name" });
            DropIndex("DMS.DictionaryAgentUsers", new[] { "UserId" });
            DropIndex("DMS.DictionaryAgentEmployees", new[] { "AgentPersonId" });
            DropIndex("DMS.DictionaryAgentEmployees", new[] { "PersonnelNumber" });
            DropIndex("DMS.DictionaryContactTypes", new[] { "Name" });
            DropIndex("DMS.DictionaryContactTypes", new[] { "Code" });
            DropIndex("DMS.DictionaryAgentContacts", new[] { "ContactTypeId" });
            DropIndex("DMS.DictionaryAgentContacts", "IX_AgentContactTypeContact");
            DropIndex("DMS.DictionaryAgentPersons", new[] { "FullName" });
            DropIndex("DMS.DictionaryAgentCompanies", new[] { "TaxCode" });
            DropIndex("DMS.DictionaryAgentCompanies", new[] { "FullName" });
            DropIndex("DMS.DictionaryAddressTypes", new[] { "Name" });
            DropIndex("DMS.DictionaryAgentAddresses", new[] { "AdressTypeId" });
            DropIndex("DMS.DictionaryAgentAddresses", "IX_AdressType");
            DropIndex("DMS.DictionaryAgentBanks", new[] { "MFOCode" });
            DropIndex("DMS.DictionaryAgentAccounts", "IX_AgentName");
            DropIndex("DMS.DictionaryAgents", new[] { "Name" });
            DropIndex("DMS.DictionaryCompanies", new[] { "Name" });
            DropIndex("DMS.DictionaryDepartments", new[] { "ParentId" });
            DropIndex("DMS.DictionaryDepartments", "IX_CompanyParentName");
            CreateIndex("DMS.CustomDictionaries", "DictionaryTypeId");
            CreateIndex("DMS.DictionaryTags", "PositionId");
            CreateIndex("DMS.DictionaryStandartSendLists", "PositionId");
            CreateIndex("DMS.DictionaryPositionExecutors", "PositionId");
            CreateIndex("DMS.DictionaryPositionExecutors", "AgentId");
            CreateIndex("DMS.DictionaryAgentContacts", "ContactTypeId");
            CreateIndex("DMS.DictionaryAgentContacts", "AgentId");
            CreateIndex("DMS.DictionaryAgentAddresses", "AdressTypeId");
            CreateIndex("DMS.DictionaryAgentAddresses", "AgentId");
            CreateIndex("DMS.DictionaryAgentAccounts", "AgentId");
            CreateIndex("DMS.DictionaryDepartments", "CompanyId");
            CreateIndex("DMS.DictionaryDepartments", "ParentId");
        }
    }
}
