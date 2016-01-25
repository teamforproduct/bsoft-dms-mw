using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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

        public virtual DbSet<DocumentAccesses> DocumentAccessesSet { get; set; }
        public virtual DbSet<DocumentEvents> DocumentEventsSet { get; set; }
        public virtual DbSet<DocumentFiles> DocumentFilesSet { get; set; }
        public virtual DbSet<DocumentIncomingDetails> DocumentIncomingDetailsSet { get; set; }
        public virtual DbSet<BL.Database.DBModel.Document.Documents> DocumentsSet { get; set; }
        public virtual DbSet<DocumentSendLists> DocumentSendListsSet { get; set; }

        public virtual DbSet<TemplateDocumentIncomingDetails> TemplateDocumentIncomingDetailsSet { get; set; }
        public virtual DbSet<TemplateDocuments> TemplateDocumentsSet { get; set; }

    }
}