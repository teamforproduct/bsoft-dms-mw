using System.Data.Entity;
using BL.Database.DBModel.Dictionary;
using BL.Database.DBModel.Admin;
using BL.Database.DBModel.Document;
using BL.Database.DBModel.System;
using BL.Database.DBModel.Template;
using System.Data.Entity.ModelConfiguration.Conventions;
using BL.CrossCutting.Helpers;

namespace BL.Database.DatabaseContext
{
    public class DmsContext : DbContext
    {

        public DmsContext() : base(ConnectionStringHelper.GetDefaultConnectionString())
        {
        }

        public DmsContext(string connString) : base(connString)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Entity<DictionaryAgents>()
            .HasOptional(f => f.AgentPerson)
            .WithRequired(s => s.Agent);

            modelBuilder.Entity<DictionaryAgents>()
            .HasOptional(f => f.AgentCompany)
            .WithRequired(s => s.Agent);

            modelBuilder.Entity<DictionaryAgents>()
            .HasOptional(f => f.AgentBank)
            .WithRequired(s => s.Agent);

            modelBuilder.Entity<DictionaryAgents>()
            .HasOptional(f => f.AgentEmployee)
            .WithRequired(s => s.Agent);

        }

        public virtual DbSet<AdminAccessLevels> AdminAccessLevelsSet { get; set; }
        public virtual DbSet<AdminRoleActions> AdminRoleActionsSet { get; set; }
        public virtual DbSet<AdminRoles> AdminRolesSet { get; set; }
        public virtual DbSet<AdminPositionRoles> AdminPositionRolesSet { get; set; }
        public virtual DbSet<AdminUserRoles> AdminUserRolesSet { get; set; }
        public virtual DbSet<AdminSubordinations> AdminSubordinationsSet { get; set; }

        public virtual DbSet<AdminLanguages> AdminLanguagesSet { get; set; }
        public virtual DbSet<AdminLanguageValues> AdminLanguageValuesSet { get; set; }

        public virtual DbSet<DictionaryAgentPersons> DictionaryAgentPersonsSet { get; set; }
        public virtual DbSet<DictionaryAgentCompanies> DictionaryAgentCompaniesSet { get; set; }
        public virtual DbSet<DictionaryAgentBanks> DictionaryAgentBanksSet { get; set; }
        public virtual DbSet<DictionaryAgentEmployees> DictionaryAgentEmployeesSet { get; set; }

        public virtual DbSet<DictionaryAgentAccounts> DictionaryAgentAccountsSet { get; set; }
        public virtual DbSet<DictionaryAgentAddresses> DictionaryAgentAddressesSet { get; set; }
        public virtual DbSet<DictionaryAgentContacts> DictionaryAgentContactsSet { get; set; }

        public virtual DbSet<DictionaryAgents> DictionaryAgentsSet { get; set; }

        public virtual DbSet<DictionaryResidentTypes> DictionaryResidentTypesSet { get; set; }
        public virtual DbSet<DictionaryContactTypes> DictionaryContactTypesSet { get; set; }
        public virtual DbSet<DictionaryAddressTypes> DictionaryAddressTypesSet { get; set; }

        public virtual DbSet<DictionaryCompanies> DictionaryCompaniesSet { get; set; }
        public virtual DbSet<DictionaryDepartments> DictionaryDepartmentsSet { get; set; }
        public virtual DbSet<DictionaryDocumentDirections> DictionaryDocumentDirectionsSet { get; set; }
        public virtual DbSet<DictionaryDocumentSubjects> DictionaryDocumentSubjectsSet { get; set; }
        public virtual DbSet<DictionaryDocumentTypes> DictionaryDocumentTypesSet { get; set; }
        public virtual DbSet<DictionaryEventTypes> DictionaryEventTypesSet { get; set; }
        public virtual DbSet<DictionaryImportanceEventTypes> DictionaryImportanceEventTypesSet { get; set; }
        public virtual DbSet<DictionaryLinkTypes> DictionaryLinkTypesSet { get; set; }
        public virtual DbSet<DictionaryPositions> DictionaryPositionsSet { get; set; }
        public virtual DbSet<DictionaryPositionExecutors> DictionaryPositionExecutorsSet { get; set; }
        public virtual DbSet<DictionaryPositionExecutorTypes> DictionaryPositionExecutorTypesSet { get; set; }
        public virtual DbSet<DictionaryRegistrationJournals> DictionaryRegistrationJournalsSet { get; set; }
        public virtual DbSet<DictionaryResultTypes> DictionaryResultTypesSet { get; set; }
        public virtual DbSet<DictionarySubscriptionStates> DictionarySubscriptionStatesSet { get; set; }
        public virtual DbSet<DictionarySendTypes> DictionarySendTypesSet { get; set; }
        public virtual DbSet<DictionaryStandartSendListContents> DictionaryStandartSendListContentsSet { get; set; }
        public virtual DbSet<DictionaryStandartSendLists> DictionaryStandartSendListsSet { get; set; }
        public virtual DbSet<DictionarySubordinationTypes> DictionarySubordinationTypesSet { get; set; }
        public virtual DbSet<DictionaryTags> DictionaryTagsSet { get; set; }

        public virtual DbSet<CustomDictionaries> CustomDictionariesSet { get; set; }
        public virtual DbSet<CustomDictionaryTypes> CustomDictionaryTypesSet { get; set; }

        public virtual DbSet<DBModel.Document.Documents> DocumentsSet { get; set; }
        public virtual DbSet<DocumentSavedFilters> DocumentSavedFiltersSet { get; set; }
        public virtual DbSet<DocumentFiles> DocumentFilesSet { get; set; }
        public virtual DbSet<DocumentLinks> DocumentLinksSet { get; set; }
        public virtual DbSet<DocumentAccesses> DocumentAccessesSet { get; set; }
        public virtual DbSet<DocumentEvents> DocumentEventsSet { get; set; }
        public virtual DbSet<DocumentRestrictedSendLists> DocumentRestrictedSendListsSet { get; set; }
        public virtual DbSet<DocumentSendLists> DocumentSendListsSet { get; set; }
        public virtual DbSet<DocumentSubscriptions> DocumentSubscriptionsSet { get; set; }
        public virtual DbSet<DocumentWaits> DocumentWaitsSet { get; set; }
        public virtual DbSet<DocumentTags> DocumentTagsSet { get; set; }
        public virtual DbSet<DocumentEventReaders> DocumentEventReadersSet { get; set; }
        public virtual DbSet<DocumentTasks> DocumentTasksSet { get; set; }

        public virtual DbSet<TemplateDocumentSendLists> TemplateDocumentSendLists { get; set; }
        public virtual DbSet<TemplateDocumentRestrictedSendLists> TemplateDocumentRestrictedSendLists { get; set; }
        public virtual DbSet<TemplateDocuments> TemplateDocumentsSet { get; set; }
        public virtual DbSet<TemplateDocumentFiles> TemplateDocumentFilesSet { get; set; }

        public virtual DbSet<SystemActions> SystemActionsSet { get; set; }
        public virtual DbSet<SystemFields> SystemFieldsSet { get; set; }
        public virtual DbSet<SystemObjects> SystemObjectsSet { get; set; }
        public virtual DbSet<SystemValueTypes> SystemValueTypesSet { get; set; }
        public virtual DbSet<SystemUIElements> SystemUIElementsSet { get; set; }

        public virtual DbSet<PropertyValues> PropertyValuesSet { get; set; }
        public virtual DbSet<PropertyLinks> PropertyLinksSet { get; set; }
        public virtual DbSet<Properties> PropertiesSet { get; set; }


        public virtual DbSet<SystemLogs> LogSet { get; set; }
        public virtual DbSet<SystemSettings> SettingsSet { get; set; }

    }
}