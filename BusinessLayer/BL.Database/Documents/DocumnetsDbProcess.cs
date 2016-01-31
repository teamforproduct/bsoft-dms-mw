using System;
using System.Collections.Generic;
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

        public void AddDocument(IContext ctx, FullDocument document)
        {
            var dbContext = GetUserDmsContext(ctx);
            var doc = _adapter.AddDocument(dbContext, document);
            dbContext.SaveChanges();
            document.Id = doc.Id;
        }

        public void UpdateDocument(IContext ctx, FullDocument document)
        {
            var dbContext = GetUserDmsContext(ctx);
            _adapter.UpdateDocument(dbContext, document);
            dbContext.SaveChanges();
        }

        public IEnumerable<FullDocument> GetDocuments(IContext ctx, FilterDocument filters)
        {
            var dbContext = GetUserDmsContext(ctx);
            var qry = dbContext.DocumentsSet.AsQueryable();

            if (filters.CreateFromDate.HasValue)
            {
                qry = qry.Where(x => x.CreateDate >= filters.CreateFromDate.Value);
            }

            if (filters.CreateToDate.HasValue)
            {
                qry = qry.Where(x => x.CreateDate <= filters.CreateToDate.Value);
            }

            if (filters.RegistrationFromDate.HasValue)
            {
                qry = qry.Where(x => x.RegistrationDate >= filters.RegistrationFromDate.Value);
            }

            if (filters.RegistrationToDate.HasValue)
            {
                qry = qry.Where(x => x.RegistrationDate <= filters.RegistrationToDate.Value);
            }

            //if (filters.SenderFromDate.HasValue)
            //{
            //    qry = qry.Where(x => x. >= filters.SenderFromDate.Value);
            //}

            //if (filters.SenderToDate.HasValue)
            //{
            //    qry = qry.Where(x => x. <= filters.SenderToDate.Value);
            //}

            if (!String.IsNullOrEmpty(filters.Description))
            {
                qry = qry.Where(x => x.Description.Contains(filters.Description));
            }

            if (!String.IsNullOrEmpty(filters.RegistrationNumber))
            {
                qry = qry.Where(x => (x.RegistrationNumberPrefix+x.RegistrationNumber.ToString()+x.RegistrationNumberSuffix).Contains(filters.RegistrationNumber));
            }

            //if (!String.IsNullOrEmpty(filters.Addressee))
            //{
            //    qry = qry.Where(x => x..Contains(filters.Addressee));
            //}

            //if (!String.IsNullOrEmpty(filters.SenderPerson))
            //{
            //    qry = qry.Where(x => x..Contains(filters.SenderPerson));
            //}

            //if (!String.IsNullOrEmpty(filters.SenderNumber))
            //{
            //    qry = qry.Where(x => x..Contains(filters.SenderNumber));
            //}

            if (filters.DocumentTypeId != null && filters.DocumentTypeId.Count > 0)
            {
                qry = qry.Where(x => filters.DocumentTypeId.Contains(x.TemplateDocument.DocumentTypeId));
            }

            if (filters.TemplateDocumentId != null && filters.TemplateDocumentId.Count > 0)
            {
                qry = qry.Where(x => filters.TemplateDocumentId.Contains(x.TemplateDocumentId));
            }

            if (filters.DocumentDirectionId != null && filters.DocumentDirectionId.Count > 0)
            {
                qry = qry.Where(x => filters.DocumentDirectionId.Contains(x.TemplateDocument.DocumentDirectionId));
            }

            if (filters.DocumentSubjectId != null && filters.DocumentSubjectId.Count > 0)
            {
                qry = qry.Where(x => x.DocumentSubjectId.HasValue && filters.DocumentSubjectId.Contains(x.DocumentSubjectId.Value));
            }

            if (filters.RegistrationJournalId != null && filters.RegistrationJournalId.Count > 0)
            {
                qry = qry.Where(x => x.RegistrationJournalId.HasValue && filters.RegistrationJournalId.Contains(x.RegistrationJournalId.Value));
            }

            if (filters.ExecutorPositionId != null && filters.ExecutorPositionId.Count > 0)
            {
                qry = qry.Where(x => filters.ExecutorPositionId.Contains(x.ExecutorPositionId));
            }

            //if (filters.SenderAgentId != null && filters.SenderAgentId.Count > 0)
            //{
            //    qry = qry.Where(x => filters.SenderAgentId.Contains(x.));
            //}


            return qry.Select(x => new FullDocument
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