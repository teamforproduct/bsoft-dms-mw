using System.Data.Entity;
using BL.Database.DBModel.Dictionary;
using BL.Database.Helpers;
using BL.Database.DBModel.Admin;
using BL.Database.DBModel.Document;
using BL.Database.DBModel.System;
using BL.Database.DBModel.Template;
using System.Data.Entity.ModelConfiguration.Conventions;
using BL.CrossCutting.Interfaces;

namespace BL.Database.DatabaseContext
{
    public class DmsContext :DbContext
    {
        private readonly IContext _context;

        internal IContext Context {
            get { return _context; }
        }

        public DmsContext() : base(ConnectionStringHelper.GetDefaultConnectionString())
        {
        }

        public DmsContext(string connString) : base(connString)
        {
            _context = null;
        }

        public DmsContext(IContext context, string connString) : base(connString)
        {
            _context = context;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            /*
            modelBuilder.Entity<DocumentIncomingDetails>()
                  .HasRequired(a => a.Document)
                  .WithMany()
                  .HasForeignKey(u => u.DocumentId);

            modelBuilder.Entity<TemplateDocumentIncomingDetails>()
                  .HasRequired(a => a.DocumentTemplate)
                  .WithMany()
                  .HasForeignKey(u => u.DocumentTemplateId);*/
        }
        


        public virtual DbSet<AdminAccessLevels> AdminAccessLevelsSet { get; set; }

        public virtual DbSet<DictionaryAgents> DictionaryAgentsSet { get; set; }
        public virtual DbSet<DictionaryCompanies> DictionaryCompaniesSet { get; set; }
        public virtual DbSet<DictionaryDepartments> DictionaryDepartmentsSet { get; set; }
        public virtual DbSet<DictionaryDocumentDirections> DictionaryDocumentDirectionsSet { get; set; }
        public virtual DbSet<DictionaryDocumentSubjects> DictionaryDocumentSubjectsSet { get; set; }
        public virtual DbSet<DictionaryDocumentTypes> DictionaryDocumentTypesSet { get; set; }
        public virtual DbSet<DictionaryEventTypes> DictionaryEventTypesSet { get; set; }
        public virtual DbSet<DictionaryImpotanceEventTypes> DictionaryImpotanceEventTypesSet { get; set; }
        public virtual DbSet<DictionaryPositions> DictionaryPositionsSet { get; set; }
        public virtual DbSet<DictionaryRegistrationJournals> DictionaryRegistrationJournalsSet { get; set; }
        public virtual DbSet<DictionarySendTypes> DictionarySendTypesSet { get; set; }
        public virtual DbSet<DictionaryStandartSendListContents> DictionaryStandartSendListContentsSet { get; set; }
        public virtual DbSet<DictionaryStandartSendLists> DictionaryStandartSendListsSet { get; set; }
        public virtual DbSet<DictionarySubordinationTypes> DictionarySubordinationTypesSet { get; set; }

        public virtual DbSet<DBModel.Document.Documents> DocumentsSet { get; set; }
        public virtual DbSet<DocumentFiles> DocumentFilesSet { get; set; }
        public virtual DbSet<DocumentIncomingDetails> DocumentIncomingDetailsSet { get; set; }
        public virtual DbSet<DocumentAccesses> DocumentAccessesSet { get; set; }
        public virtual DbSet<DocumentEvents> DocumentEventsSet { get; set; }
        public virtual DbSet<DocumentRestrictedSendLists> DocumentRestrictedSendListsSet { get; set; }
        public virtual DbSet<DocumentSendLists> DocumentSendListsSet { get; set; }

        public virtual DbSet<TemplateDocumentSendLists> TemplateDocumentSendLists { get; set; }
        public virtual DbSet<TemplateDocumentRestrictedSendLists> TemplateDocumentRestrictedSendLists { get; set; }
        public virtual DbSet<TemplateDocumentIncomingDetails> TemplateDocumentIncomingDetailsSet { get; set; }
        public virtual DbSet<TemplateDocuments> TemplateDocumentsSet { get; set; }

        public virtual DbSet<SystemLogs> LogSet { get; set; }
        public virtual DbSet<SystemSettings> SettingsSet { get; set; }

    }
}