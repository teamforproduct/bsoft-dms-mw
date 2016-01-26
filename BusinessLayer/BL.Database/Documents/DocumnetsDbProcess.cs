﻿using System;
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
            dbContext.DocumentsSet.Add(new DBModel.Document.Documents
            {
                CreateDate = DateTime.Now,
                Description = document.Description,
                Files = null,
                LastChangeDate = DateTime.Now,
                TemplateDocumentId = null,
                RegistrationDate = null,
                RegistrationNumber = 1,
                DocumentSubjectId = null,
                RestrictedSendListId = null,
                RegistrationJournalId = document.RegistrationJournalId,
                RegistrationNumberPrefix = "AA",
                RegistrationNumberSuffix = "SS",
                DocumentDirectionId = document.DocumentDirectionId,
                ExecutorPositionId = document.ExecutorPositionId,
                DocumentTypeId = document.DocumentTypeId
                
            });
            dbContext.SaveChanges();
        }

        public void UpdateDocument(IContext ctx, BaseDocument document)
        {
            var dbContext = GetUserDmsContext(ctx);
        }

        public IEnumerable<BaseDocument> GetDocuments(IContext ctx)
        {
            var dbContext = GetUserDmsContext(ctx);

            return dbContext.DocumentsSet.Select(x => new BaseDocument
            {
                Id = x.Id,
                DocumentTypeId = x.DocumentTypeId,
                ExecutorPositionId = x.ExecutorPositionId,
                DocumentDirectionId = x.DocumentDirectionId,
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
                DocumentSubject = x.DocumentSubject.Name,
                ExecutorName = x.ExecutorPosition.Name,
                LastChangeUserId = x.LastChangeUserId,
                DocumentDirection = x.DocumentDirection.Name
            }).ToList();
        }

        public BaseDocument GetDocument(IContext ctx, int documentId)
        {
            var dbContext = GetUserDmsContext(ctx);

            return dbContext.DocumentsSet.Where(x=>x.Id == documentId).Select(x => new BaseDocument
            {
                Id = x.Id,
                DocumentTypeId = x.DocumentTypeId,
                ExecutorPositionId = x.ExecutorPositionId,
                DocumentDirectionId = x.DocumentDirectionId,
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
                DocumentSubject = x.DocumentSubject.Name,
                ExecutorName = x.ExecutorPosition.Name,
                LastChangeUserId = x.LastChangeUserId,
                DocumentDirection = x.DocumentDirection.Name
            }).FirstOrDefault();
        }
    }
}