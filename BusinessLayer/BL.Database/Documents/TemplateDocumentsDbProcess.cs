using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;
using System.Linq;
using BL.CrossCutting.Helpers;
using BL.Database.DatabaseContext;
using BL.Database.Documents.Interfaces;
using BL.Model.Enums;

namespace BL.Database.Documents
{
    public class TemplateDocumentsDbProcess : CoreDb.CoreDb, ITemplateDocumentsDbProcess
    {
        private readonly IConnectionStringHelper _helper;

        public TemplateDocumentsDbProcess(IConnectionStringHelper helper)
        {
            _helper = helper;
        }

        public IEnumerable<BaseTemplateDocument> GetTemplateDocument(IContext ctx)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                return dbContext.TemplateDocumentsSet.Select(x => new BaseTemplateDocument
                {
                    Id = x.Id,
                    DocumentDirection = (EnumDocumentDirections) x.DocumentDirectionId,
                    DocumentTypeId = x.DocumentTypeId,
                    Name = x.Name,
                    Description = x.Description,
                    DocumentSubjectId = x.DocumentSubjectId,
                    LastChangeDate = x.LastChangeDate,
                    RegistrationJournalId = x.RegistrationJournalId,
                    DocumentSubjectName = x.DocumentSubject.Name,
                    LastChangeUserId = x.LastChangeUserId,
                    DocumentDirectionName = x.DocumentDirection.Name
                }).ToList();
            }
        }

        public BaseTemplateDocument GetTemplateDocumentByDocumentId(IContext ctx, int documentId)
        {
            int templateDocumentId = 0;
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                templateDocumentId = dbContext.DocumentsSet.Where(x => x.Id == documentId).Select(x => x.TemplateDocumentId).FirstOrDefault();
            }

            return GetTemplateDocument(ctx, templateDocumentId);
        }

        public BaseTemplateDocument GetTemplateDocument(IContext ctx, int templateDocumentId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                return
                    dbContext.TemplateDocumentsSet.Where(x => x.Id == templateDocumentId)
                        .Select(x => new BaseTemplateDocument
                        {
                            Id = x.Id,
                            Name = x.Name,
                            IsHard = x.IsHard,
                            DocumentDirection = (EnumDocumentDirections) x.DocumentDirectionId,
                            DocumentTypeId = x.DocumentTypeId,
                            Description = x.Description,
                            DocumentSubjectId = x.DocumentSubjectId,
                            RegistrationJournalId = x.RegistrationJournalId,
                            SenderAgentId = x.SenderAgentId,
                            SenderAgentPersonId = x.SenderAgentPersonId,
                            Addressee = x.Addressee,
                            LastChangeUserId = x.LastChangeUserId,
                            LastChangeDate = x.LastChangeDate,
                            RestrictedSendLists =
                                x.RestrictedSendLists.Select(y => new BaseTemplateDocumentRestrictedSendLists()
                                {
                                    PositionId = y.PositionId,
                                    AccessLevel = (EnumDocumentAccesses) y.AccessLevelId
                                }).ToList(),
                            SendLists = x.SendLists.Select(y => new BaseTemplateDocumentSendLists()
                            {
                                SendType = (EnumSendTypes) y.SendTypeId,
                                TargetPositionId = y.TargetPositionId,
                                Description = y.Description,
                                DueDate = y.DueDate,
                                Stage = y.Stage,

                                DueDay = y.DueDay,
                                AccessLevel = (EnumDocumentAccesses) y.AccessLevelId
                            }).ToList()
                        }).FirstOrDefault();
            }
        }

        public int AddOrUpdateTemplate(IContext ctx, BaseTemplateDocument template)
        {
            // we should not implement it now
            //var dbContext = GetUserDmsContext(context);

            return 0;
        }
    }
}