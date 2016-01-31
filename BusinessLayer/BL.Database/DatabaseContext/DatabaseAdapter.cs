using System;
using System.Linq;
using BL.Model.DocumentCore;

namespace BL.Database.DatabaseContext
{
    public class DatabaseAdapter : IDatabaseAdapter
    {
        public DBModel.Document.Documents AddDocument(DmsContext dbContext, FullDocument document)
        {
            var doc = new DBModel.Document.Documents
            {
                TemplateDocumentId = document.TemplateDocumentId,
                CreateDate = document.CreateDate,
                DocumentSubjectId = document.DocumentSubjectId,
                Description = document.Description,
                RegistrationJournalId = document.RegistrationJournalId,
                RegistrationNumberSuffix = document.RegistrationNumberSuffix,
                RegistrationNumberPrefix = document.RegistrationNumberPrefix,
                RegistrationDate = document.RegistrationDate,
                ExecutorPositionId = document.ExecutorPositionId,
                ExecutorAgentId = document.ExecutorAgentId,
                LastChangeUserId = dbContext.Context.CurrentAgentId,
                LastChangeDate = DateTime.Now
            };
            dbContext.DocumentsSet.Add(doc);
            return doc;
        }

        public void UpdateDocument(DmsContext dbContext, FullDocument document)
        {
            var doc = dbContext.DocumentsSet.FirstOrDefault(x => x.Id == document.Id);
            if (doc != null)
            {
                doc.TemplateDocumentId = document.TemplateDocumentId;
                doc.DocumentSubjectId = document.DocumentSubjectId;
                doc.Description = document.Description;
                doc.RegistrationJournalId = document.RegistrationJournalId;
                doc.RegistrationNumberSuffix = document.RegistrationNumberSuffix;
                doc.RegistrationNumberPrefix = document.RegistrationNumberPrefix;
                doc.RegistrationDate = document.RegistrationDate;
                doc.ExecutorPositionId = document.ExecutorPositionId;
                doc.ExecutorAgentId = document.ExecutorAgentId;
                doc.LastChangeUserId = dbContext.Context.CurrentAgentId;
                doc.LastChangeDate = DateTime.Now;
            }
        }

        public int AddTemplate(DmsContext dbContext, BaseTemplateDocument template)
        {
            return 0;
        }
    }
}