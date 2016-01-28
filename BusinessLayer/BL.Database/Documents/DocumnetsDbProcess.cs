using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Database.DatabaseContext;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore;

namespace BL.Database.Documents
{
    internal class DocumnetsDbProcess : CoreDb.CoreDb, IDocumnetsDbProcess
    {
        private readonly IDatabaseAdapter _adapter;

        public DocumnetsDbProcess(IDatabaseAdapter adapter)
        {
            _adapter = adapter;
        }

        public void AddDocument(IContext ctx, BaseDocument document)
        {
            var dbContext = GetUserDmsContext(ctx);
            _adapter.AddDocument(dbContext, document);
            dbContext.SaveChanges();
            // here we should update doc ID 
        }

        public void UpdateDocument(IContext ctx, BaseDocument document)
        {
            var dbContext = GetUserDmsContext(ctx);
            _adapter.UpdateDocument(dbContext, document);
            dbContext.SaveChanges();
        }

        public IEnumerable<BaseDocument> GetDocuments(IContext ctx, FilterDocument filters)
        {
            var dbContext = GetUserDmsContext(ctx);

            return dbContext.DocumentsSet.Select(x => new BaseDocument
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

        public BaseDocument GetDocument(IContext ctx, int documentId)
        {
            var dbContext = GetUserDmsContext(ctx);

            return dbContext.DocumentsSet.Where(x=>x.Id == documentId).Select(x => new BaseDocument
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