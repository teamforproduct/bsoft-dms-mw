using System;
using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;

namespace BL.Database.Documents
{
    internal class DocumnetsDbProcess : CoreDb.CoreDb, IDocumnetsDbProcess
    {
        public void AddDocument(IContext ctx, BaseDocument document)
        {
            var dbContext = GetUserDmsContext(ctx);
            var doc= new DBModel.Document.Documents
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
            dbContext.SaveChanges();
            document.Id = doc.Id;
        }

        public void UpdateDocument(IContext ctx, BaseDocument document)
        {
            var dbContext = GetUserDmsContext(ctx);
            var doc = dbContext.DocumentsSet.FirstOrDefault(x => x.Id == document.Id);
            if (doc != null)
            {
                doc.Description = document.Description;
                doc.LastChangeDate = DateTime.Now;
                doc.TemplateDocumentId = document.TemplateDocumentId;
                doc.ExecutorAgentId = document.ExecutorAgentId;
                doc.ExecutorPositionId = document.ExecutorPositionId;
                doc.RestrictedSendListId = document.RestrictedSendListId;
                dbContext.SaveChanges();
            }
        }

        public IEnumerable<FullDocument> GetDocuments(IContext ctx, DocumentFilter filters)
        {
            var dbContext = GetUserDmsContext(ctx);

            return dbContext.DocumentsSet.Select(x => new FullDocument
            {
                    Id = x.Id,
                    DocumentTypeId = x.TemplateDocument.DocumentTypeId,
                    ExecutorPositionId = x.ExecutorPositionId,
                    DocumentDirectionId = x.TemplateDocument.DocumentDirectionId,
                    Description = x.Description,
                    TemplateDocumentId = x.TemplateDocumentId,
                    RegistrationDate = x.RegistrationDate,
                    DocumentSubjectId = x.DocumentSubjectId,
                    RegistrationNumber = x.RegistrationNumber,
                    RegistrationNumberPrefix = x.RegistrationNumberPrefix,
                    RegistrationNumberSuffix = x.RegistrationNumberSuffix,
                    LastChangeDate = x.LastChangeDate,
                    RegistrationJournalId = x.RegistrationJournalId,
                    CreateDate = x.CreateDate,
                    DocumentSubjectName = x.DocumentSubject.Name,
                    ExecutorPositionName = x.ExecutorPosition.Name,
                    LastChangeUserId = x.LastChangeUserId,
                    DocumentDirectionName = x.TemplateDocument.DocumentDirection.Name
                }).ToList();
        }

        public FullDocument GetDocument(IContext ctx, int documentId)
        {
            var dbContext = GetUserDmsContext(ctx);

            return dbContext.DocumentsSet.Where(x=>x.Id == documentId).Select(x => new FullDocument
            {
                Id = x.Id,
                DocumentTypeId = x.TemplateDocument.DocumentTypeId,
                ExecutorPositionId = x.ExecutorPositionId,
                DocumentDirectionId = x.TemplateDocument.DocumentDirectionId,
                Description = x.Description,
                TemplateDocumentId = x.TemplateDocumentId,
                RegistrationDate = x.RegistrationDate,
                DocumentSubjectId = x.DocumentSubjectId,
                RegistrationNumber = x.RegistrationNumber,
                RegistrationNumberPrefix = x.RegistrationNumberPrefix,
                RegistrationNumberSuffix = x.RegistrationNumberSuffix,
                LastChangeDate = x.LastChangeDate,
                RegistrationJournalId = x.RegistrationJournalId,
                CreateDate = x.CreateDate,
                DocumentSubjectName = x.DocumentSubject.Name,
                ExecutorPositionName = x.ExecutorPosition.Name,
                LastChangeUserId = x.LastChangeUserId,
                DocumentDirectionName = x.TemplateDocument.DocumentDirection.Name
            }).FirstOrDefault();
        }
    }
}