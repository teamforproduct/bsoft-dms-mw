﻿using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.Common;
using BL.Database.DBModel.Admin;
using BL.Database.DBModel.Dictionary;
using BL.Database.DBModel.Document;
using BL.Database.DBModel.Encryption;
using BL.Database.DBModel.System;
using BL.Database.DBModel.Template;
using BL.Database.Helper;
using BL.Model.Context;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace BL.Database.DatabaseContext
{

    public class DmsContext : DbContext, IDmsDatabaseContext
    {

        private string _DefaultSchema { get; set; }
        //TODO remove in release version
        public DmsContext() : base(ConnectionHelper.GetDefaultConnection(), true)
        {
            _DefaultSchema = ConnectionHelper.GetDefaultSchema();

            ((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized +=
                    (sender, e) => DateTimeKindAttribute.Apply(e.Entity);
        }

        /////////////////////////////////////////////////////////////
        // ПАРАМЕТР dbModel НИ В КОЕМ СЛУЧАЕ НЕ ПЕРЕИМЕНОВЫВАТЬ!!! //
        // DbContext = DmsResolver.Current.Kernel.Get<IDmsDatabaseContext>(new ConstructorArgument("dbModel", CurrentDB)); //
        /////////////////////////////////////////////////////////////
        public DmsContext(DatabaseModel dbModel): base(DmsResolver.Current.Get<ConnectionHelper>().GetConnection(dbModel), true)
        {
            _DefaultSchema = dbModel.DefaultSchema;

            this.Database.CommandTimeout = int.MaxValue;
            System.Data.Entity.Database.SetInitializer<DmsContext>(new DmsDbInitializer());
            if (!this.Database.Exists())
            {
                this.Database.Initialize(true);
            }
        }

        public DmsContext(IContext context) : this(context.CurrentDB)
        {
        }

        public void SafeAttach<T>(T entity) where T : class
        {
            foreach (var entry in ChangeTracker.Entries<T>())
            {
                ((IObjectContextAdapter)this).ObjectContext.Detach(entry.Entity);
            }
            Set<T>().Attach(entity);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(_DefaultSchema);
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Entity<DictionaryAgents>()
            .HasOptional(f => f.AgentPerson)
            .WithRequired(s => s.Agent);

            modelBuilder.Entity<DictionaryAgents>()
            .HasOptional(f => f.AgentPeople)
            .WithRequired(s => s.Agent);

            modelBuilder.Entity<DictionaryAgents>()
            .HasOptional(f => f.AgentUser)
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

            modelBuilder.Entity<DictionaryAgents>()
            .HasOptional(f => f.AgentOrg)
            .WithRequired(s => s.Agent);

        }

        public virtual DbSet<AdminAccessLevels> AdminAccessLevelsSet { get; set; }
        //public virtual DbSet<AdminRoleActions> AdminRoleActionsSet { get; set; }
        public virtual DbSet<AdminRolePermissions> AdminRolePermissionsSet { get; set; }
        public virtual DbSet<AdminRoles> AdminRolesSet { get; set; }
        public virtual DbSet<AdminRoleTypes> AdminRolesTypesSet { get; set; }
        public virtual DbSet<AdminPositionRoles> AdminPositionRolesSet { get; set; }
        public virtual DbSet<AdminUserRoles> AdminUserRolesSet { get; set; }
        public virtual DbSet<AdminEmployeeDepartments> AdminEmployeeDepartmentsSet { get; set; }
        public virtual DbSet<AdminSubordinations> AdminSubordinationsSet { get; set; }
        public virtual DbSet<AdminRegistrationJournalPositions> AdminRegistrationJournalPositionsSet { get; set; }


        public virtual DbSet<DictionaryAgents> DictionaryAgentsSet { get; set; }

        public virtual DbSet<DictionaryAgentPersons> DictionaryAgentPersonsSet { get; set; }

        public virtual DbSet<DictionaryAgentPeople> DictionaryAgentPeopleSet { get; set; }
        public virtual DbSet<DictionaryAgentUsers> DictionaryAgentUsersSet { get; set; }
        public virtual DbSet<DictionaryAgentCompanies> DictionaryAgentCompaniesSet { get; set; }
        public virtual DbSet<DictionaryAgentBanks> DictionaryAgentBanksSet { get; set; }
        public virtual DbSet<DictionaryAgentEmployees> DictionaryAgentEmployeesSet { get; set; }

        public virtual DbSet<DictionaryAgentAccounts> DictionaryAgentAccountsSet { get; set; }
        public virtual DbSet<DictionaryAgentAddresses> DictionaryAgentAddressesSet { get; set; }
        public virtual DbSet<DictionaryAgentContacts> DictionaryAgentContactsSet { get; set; }
        public virtual DbSet<DictionaryAgentFavorites> DictionaryAgentFavoritesSet { get; set; }
        

        public virtual DbSet<DictionaryResidentTypes> DictionaryResidentTypesSet { get; set; }
        public virtual DbSet<DictionaryContactTypes> DictionaryContactTypesSet { get; set; }
        public virtual DbSet<DictionaryAddressTypes> DictionaryAddressTypesSet { get; set; }

        public virtual DbSet<DictionaryCompanies> DictionaryAgentClientCompaniesSet { get; set; }
        public virtual DbSet<DictionaryDepartments> DictionaryDepartmentsSet { get; set; }
        public virtual DbSet<DictionaryDocumentDirections> DictionaryDocumentDirectionsSet { get; set; }
        public virtual DbSet<DictionaryDocumentSubjects> DictionaryDocumentSubjectsSet { get; set; }
        public virtual DbSet<DictionaryDocumentTypes> DictionaryDocumentTypesSet { get; set; }
        public virtual DbSet<DictionaryEventTypes> DictionaryEventTypesSet { get; set; }
        public virtual DbSet<DictionaryImportanceEventTypes> DictionaryImportanceEventTypesSet { get; set; }
        public virtual DbSet<DictionaryLinkTypes> DictionaryLinkTypesSet { get; set; }
        public virtual DbSet<DictionaryFileTypes> DictionaryFileTypesSet { get; set; }
        public virtual DbSet<DictionarySigningTypes> DictionarySigningTypesSet { get; set; }
        public virtual DbSet<DictionaryPositions> DictionaryPositionsSet { get; set; }
        public virtual DbSet<DictionaryPositionExecutors> DictionaryPositionExecutorsSet { get; set; }
        public virtual DbSet<DictionaryPositionExecutorTypes> DictionaryPositionExecutorTypesSet { get; set; }
        public virtual DbSet<DictionaryRegistrationJournals> DictionaryRegistrationJournalsSet { get; set; }
        public virtual DbSet<DicRegJournalAccessTypes> DictionaryRegistrationJournalAccessTypesSet { get; set; }
        public virtual DbSet<DictionaryResultTypes> DictionaryResultTypesSet { get; set; }
        public virtual DbSet<DictionarySubscriptionStates> DictionarySubscriptionStatesSet { get; set; }
        public virtual DbSet<DictionarySendTypes> DictionarySendTypesSet { get; set; }
        public virtual DbSet<DictionaryStageTypes> DictionaryStageTypesSet { get; set; }
        public virtual DbSet<DictionaryStandartSendListContents> DictionaryStandartSendListContentsSet { get; set; }
        public virtual DbSet<DictionaryStandartSendLists> DictionaryStandartSendListsSet { get; set; }
        public virtual DbSet<DictionarySubordinationTypes> DictionarySubordinationTypesSet { get; set; }
        public virtual DbSet<DictionaryTags> DictionaryTagsSet { get; set; }
        public virtual DbSet<DictionarySettingTypes> DictionarySettingTypesSet { get; set; }

        public virtual DbSet<CustomDictionaries> CustomDictionariesSet { get; set; }
        public virtual DbSet<CustomDictionaryTypes> CustomDictionaryTypesSet { get; set; }

        public virtual DbSet<DBModel.Document.Documents> DocumentsSet { get; set; }
        public virtual DbSet<DocumentSavedFilters> DocumentSavedFiltersSet { get; set; }
        public virtual DbSet<DocumentFiles> DocumentFilesSet { get; set; }
        public virtual DbSet<DocumentFileLinks> DocumentFileLinksSet { get; set; }
        public virtual DbSet<DocumentLinks> DocumentLinksSet { get; set; }
        public virtual DbSet<DocumentAccesses> DocumentAccessesSet { get; set; }
        public virtual DbSet<DocumentEvents> DocumentEventsSet { get; set; }
        public virtual DbSet<DocumentRestrictedSendLists> DocumentRestrictedSendListsSet { get; set; }
        public virtual DbSet<DocumentSendLists> DocumentSendListsSet { get; set; }
        public virtual DbSet<DocumentSendListAccessGroups> DocumentSendListAccessGroupsSet { get; set; }
        public virtual DbSet<DocumentSubscriptions> DocumentSubscriptionsSet { get; set; }
        public virtual DbSet<DocumentWaits> DocumentWaitsSet { get; set; }
        public virtual DbSet<DocumentTags> DocumentTagsSet { get; set; }
        public virtual DbSet<DocumentEventAccesses> DocumentEventAccessesSet { get; set; }
        public virtual DbSet<DocumentEventAccessGroups> DocumentEventAccessGroupsSet { get; set; }
        public virtual DbSet<DocumentTasks> DocumentTasksSet { get; set; }
        public virtual DbSet<DocumentTaskAccesses> DocumentTaskAccessesSet { get; set; }

        public virtual DbSet<DocumentPapers> DocumentPapersSet { get; set; }
        public virtual DbSet<DocumentPaperLists> DocumentPaperListsSet { get; set; }

        public virtual DbSet<TemplateDocumentSendLists> TemplateDocumentSendListsSet { get; set; }
        public virtual DbSet<TemplateDocumentSendListAccessGroups> TemplateDocumentSendListAccessGroupsSet { get; set; }
        public virtual DbSet<TemplateDocumentRestrictedSendLists> TemplateDocumentRestrictedSendListsSet { get; set; }
        public virtual DbSet<TemplateAccesses> TemplateDocumentAccessesSet { get; set; }
        public virtual DbSet<TemplateDocuments> TemplateDocumentsSet { get; set; }
        public virtual DbSet<TemplateDocumentFiles> TemplateDocumentFilesSet { get; set; }
        public virtual DbSet<TemplateDocumentTasks> TemplateDocumentTasksSet { get; set; }
        public virtual DbSet<TemplateDocumentPapers> TemplateDocumentPapersSet { get; set; }



        
        public virtual DbSet<SystemModules> SystemModulesSet { get; set; }
        public virtual DbSet<SystemFeatures> SystemFeaturesSet { get; set; }
        public virtual DbSet<SystemAccessTypes> SystemAccessTypesSet { get; set; }
        public virtual DbSet<SystemPermissions> SystemPermissionsSet { get; set; }
        public virtual DbSet<SystemActions> SystemActionsSet { get; set; }
        public virtual DbSet<SystemFields> SystemFieldsSet { get; set; }
        public virtual DbSet<SystemObjects> SystemObjectsSet { get; set; }
        public virtual DbSet<SystemValueTypes> SystemValueTypesSet { get; set; }
        public virtual DbSet<SystemUIElements> SystemUIElementsSet { get; set; }
        public virtual DbSet<SystemMenus> SystemMenusSet { get; set; }

        public virtual DbSet<SystemPatterns> SystemPatternsSet { get; set; }
        public virtual DbSet<SystemFormulas> SystemFormulasSet { get; set; }
        public virtual DbSet<SystemFormats> SystemFormatsSet { get; set; }

        public virtual DbSet<PropertyValues> PropertyValuesSet { get; set; }
        public virtual DbSet<PropertyLinks> PropertyLinksSet { get; set; }
        public virtual DbSet<Properties> PropertiesSet { get; set; }

        public virtual DbSet<FullTextIndexCash> FullTextIndexCashSet { get; set; }
        public virtual DbSet<SystemLogs> LogSet { get; set; }
        public virtual DbSet<SystemSearchQueryLogs> SystemSearchQueryLogsSet { get; set; }
        public virtual DbSet<SystemSettings> SettingsSet { get; set; }

        public virtual DbSet<EncryptionCertificates> EncryptionCertificatesSet { get; set; }
    }
}