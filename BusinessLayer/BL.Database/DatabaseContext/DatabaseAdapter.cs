using System;
using System.Linq;
using BL.Model.DocumentCore;

namespace BL.Database.DatabaseContext
{
    public class DatabaseAdapter : IDatabaseAdapter
    {
        public void AddDocument(DmsContext dbContext, BaseDocument document)
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
                LastChangeUserId = 0,
                LastChangeDate = DateTime.Now
            };
            dbContext.DocumentsSet.Add(doc);
        }

        public void UpdateDocument(DmsContext dbContext, BaseDocument document)
        {
            var doc = dbContext.DocumentsSet.FirstOrDefault(x => x.Id == document.Id);
            if (doc != null)
            {
                doc.TemplateDocumentId = document.TemplateDocumentId;
                doc.CreateDate = document.CreateDate;
                doc.DocumentSubjectId = document.DocumentSubjectId;
                doc.Description = document.Description;
                doc.RegistrationJournalId = document.RegistrationJournalId;
                doc.RegistrationNumberSuffix = document.RegistrationNumberSuffix;
                doc.RegistrationNumberPrefix = document.RegistrationNumberPrefix;
                doc.RegistrationDate = document.RegistrationDate;
                doc.ExecutorPositionId = document.ExecutorPositionId;
                doc.ExecutorAgentId = document.ExecutorAgentId;
                doc.LastChangeUserId = 0;
                doc.LastChangeDate = DateTime.Now;
            }
        }

        public int AddTemplate(DmsContext dbContext, BaseTemplateDocument template)
        {
            return 0;
        }
    }
}