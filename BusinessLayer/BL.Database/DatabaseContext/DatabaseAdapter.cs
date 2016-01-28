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
                CreateDate = DateTime.Now,
                Description = document.Description,
                ExecutorAgentId = document.ExecutorAgentId,
                RestrictedSendListId = document.RestrictedSendListId,
                LastChangeDate = DateTime.Now,
                TemplateDocumentId = document.TemplateDocumentId,
                RegistrationNumber = 1,
                //DocumentDirectionId = document.DocumentDirectionId,
                ExecutorPositionId = document.ExecutorPositionId,
                //DocumentTypeId = document.DocumentTypeId

            };
            dbContext.DocumentsSet.Add(doc);
        }

        public void UpdateBaseDocument(DmsContext dbContext, BaseDocument document)
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

        public int AddTemplate(DmsContext dbContext, FullTemplateDocument template)
        {
            return 0;
        }
    }
}