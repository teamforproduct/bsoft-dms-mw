using System.Data.Entity;
using BL.Database.DBModel.Dictionary;
using BL.Database.Helpers;
using BL.Database.DBModel.Admin;
using BL.Database.DBModel.Document;
using BL.Database.DBModel.Template;

namespace BL.Database.DatabaseContext
{
    public class DmsContext :DbContext
    {
        public DmsContext() : base(ConnectionStringHelper.GetDefaultConnectionString())
        {
        }

        public DmsContext(string connString) : base(connString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DocumentIncomingDetails>()
                  .HasRequired(a => a.Document)
                  .WithMany()
                  .HasForeignKey(u => u.DocumentId);

            modelBuilder.Entity<TemplateDocumentIncomingDetails>()
                  .HasRequired(a => a.DocumentTemplate)
                  .WithMany()
                  .HasForeignKey(u => u.DocumentTemplateId);
        }

        public virtual DbSet<DBModel.Document.Documents> DocumentsSet { get; set; }
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
        public virtual DbSet<DictionaryStandartSendListContents> DictionaryStandartSendListContentsSet { get; set; }
        public virtual DbSet<DictionaryStandartSendLists> DictionaryStandartSendListsSet { get; set; }
        public virtual DbSet<DictionarySubordinationTypes> DictionarySubordinationTypesSet { get; set; }

        public virtual DbSet<DocumentFiles> DocumentFilesSet { get; set; }
        public virtual DbSet<DocumentIncomingDetails> DocumentIncomingDetailsSet { get; set; }


        public virtual DbSet<TemplateDocumentIncomingDetails> TemplateDocumentIncomingDetailsSet { get; set; }
        public virtual DbSet<TemplateDocuments> TemplateDocumentsSet { get; set; }

    }
}