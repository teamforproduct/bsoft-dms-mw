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
                RestrictedSendListId = document.RestrictedSendListId,
                RegistrationNumberSuffix = document.RegistrationNumberSuffix,
                RegistrationNumberPrefix = document.RegistrationNumberPrefix,
                RegistrationDate = document.RegistrationDate,
                ExecutorPositionId = document.ExecutorPositionId,
                ExecutorAgentId = document.ExecutorAgentId,
                LastChangeUserId = 0,
                LastChangeDate = DateTime.Now

                //DocumentDirectionId = document.DocumentDirectionId,

                //DocumentTypeId = document.DocumentTypeId

            };
            dbContext.DocumentsSet.Add(doc);
        }

        public void UpdateDocument(DmsContext dbContext, BaseDocument document)
        {
            var doc = dbContext.DocumentsSet.FirstOrDefault(x => x.Id == document.Id);
            if (doc != null)
            {
                doc.Description = document.Description;
                doc.LastChangeDate = DateTime.Now;
                doc.TemplateDocumentId = document.TemplateDocumentId;
                doc.ExecutorAgentId = document.ExecutorAgentId;
                doc.ExecutorPositionId = document.ExecutorPositionId;
                doc.RestrictedSendListId = document.RestrictedSendListId;
            }
        }

        public int AddTemplate(DmsContext dbContext, BaseTemplateDocument template)
        {
            return 0;
        }
    }
}