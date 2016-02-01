using System;
using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore;
using BL.Database.DBModel.Document;

namespace BL.Database.Documents
{
    internal class DocumnetsDbProcess : CoreDb.CoreDb, IDocumnetsDbProcess
    {
        public DocumnetsDbProcess()
        {
        }
        #region Documents
        public void AddDocument(IContext ctx, FullDocument document)
        {
            var dbContext = GetUserDmsContext(ctx);

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
            if (document.RestrictedSendLists != null && document.RestrictedSendLists.Count > 0)
            {
                doc.RestrictedSendLists = document.RestrictedSendLists.Select(x => new DocumentRestrictedSendLists()
                {
                    PositionId = x.PositionId,
                    AccessLevelId = x.AccessLevelId,
                    LastChangeUserId = dbContext.Context.CurrentAgentId,
                    LastChangeDate = DateTime.Now
                }).ToList();
            }
            dbContext.DocumentsSet.Add(doc);
            dbContext.SaveChanges();
            document.Id = doc.Id;
        }

        public void UpdateDocument(IContext ctx, FullDocument document)
        {
            var dbContext = GetUserDmsContext(ctx);
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
        #endregion Documents

        #region DocumentRestrictedSendLists
        public int AddRestrictedSendList(IContext ctx, ModifyDocumentRestrictedSendList restrictedSendList)
        {
            var dbContext = GetUserDmsContext(ctx);

            var sendList = new DBModel.Document.DocumentRestrictedSendLists
            {
                DocumentId = restrictedSendList.DocumentId,
                PositionId = restrictedSendList.PositionId,
                AccessLevelId = restrictedSendList.AccessLevelId,
                LastChangeUserId = dbContext.Context.CurrentAgentId,
                LastChangeDate = DateTime.Now
            };
            dbContext.DocumentRestrictedSendListsSet.Add(sendList);
            dbContext.SaveChanges();
            return sendList.Id;
        }

        public void DeleteRestrictedSendList(IContext ctx, int restrictedSendListId)
        {
            var dbContext = GetUserDmsContext(ctx);
            var sendList = dbContext.DocumentRestrictedSendListsSet.FirstOrDefault(x => x.Id == restrictedSendListId);
            if (sendList != null)
            {
                dbContext.DocumentRestrictedSendListsSet.Remove(sendList);
                dbContext.SaveChanges();
            }
        }
        #endregion DocumentRestrictedSendLists
    }
}