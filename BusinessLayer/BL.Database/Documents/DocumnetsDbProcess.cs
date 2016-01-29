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
                TemplateDocumentId = x.TemplateDocumentId,
                CreateDate = x.CreateDate,
                DocumentSubjectId = x.DocumentSubjectId,
                Description = x.Description,
                RegistrationJournalId = x.RegistrationJournalId,
                RegistrationNumber = x.RegistrationNumber,
                RegistrationNumberPrefix = x.RegistrationNumberPrefix,
                RegistrationNumberSuffix = x.RegistrationNumberSuffix,
                RegistrationDate = x.RegistrationDate,
                ExecutorPositionId = x.ExecutorPositionId,
                ExecutorAgentId = x.ExecutorAgentId,
                LastChangeUserId = x.LastChangeUserId,
                LastChangeDate = x.LastChangeDate,
                SenderAgentId = x.IncomingDetail.SenderAgentId,
                SenderPerson = x.IncomingDetail.SenderPerson,
                SenderNumber = x.IncomingDetail.SenderNumber,
                SenderDate = x.IncomingDetail.SenderDate,
                Addressee = x.IncomingDetail.Addressee,
                AccessLevelId = 30, //после добавления Access??? подумать
                TemplateDocumentName = x.TemplateDocument.Name,
                IsHard = x.TemplateDocument.IsHard,
                DocumentDirectionId = x.TemplateDocument.DocumentDirectionId,
                DocumentDirectionName = x.TemplateDocument.DocumentDirection.Name,
                DocumentTypeId = x.TemplateDocument.DocumentTypeId,
                DocumentTypeName = x.TemplateDocument.DocumentType.Name,
                DocumentSubjectName = x.DocumentSubject.Name,
                RegistrationJournalName = x.RegistrationJournal.Name,
                ExecutorPositionName = x.ExecutorPosition.Name,
                ExecutorAgentName = x.ExecutorAgent.Name,
                SenderAgentName = x.IncomingDetail.SenderAgent.Name,
                AccessLevelName = null //после добавления Access??? подумать
            }).FirstOrDefault();
        }
    }
}