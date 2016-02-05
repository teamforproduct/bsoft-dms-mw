using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;
using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Model.Enums;

namespace BL.Database.Documents
{
    public class TemplateDocumentsDbProcess : CoreDb.CoreDb, ITemplateDocumentsDbProcess
    {
        public IEnumerable<BaseTemplateDocument> GetTemplateDocument(IContext context)
        {
            var dbContext = GetUserDmsContext(context);
            return dbContext.TemplateDocumentsSet.Select(x => new BaseTemplateDocument
            {
                Id = x.Id,
                DocumentDirection = (EnumDocumentDirections)x.DocumentDirectionId,
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

        public BaseTemplateDocument GetTemplateDocument(IContext context, int templateDocumentId)
        {
            var dbContext = GetUserDmsContext(context);
            return dbContext.TemplateDocumentsSet.Where(x => x.Id == templateDocumentId).Select(x => new BaseTemplateDocument
            {
                Id = x.Id,
                Name = x.Name,
                IsHard = x.IsHard,
                DocumentDirection = (EnumDocumentDirections)x.DocumentDirectionId,
                DocumentTypeId = x.DocumentTypeId,
                Description = x.Description,
                DocumentSubjectId = x.DocumentSubjectId,
                RegistrationJournalId = x.RegistrationJournalId,
                LastChangeUserId = x.LastChangeUserId,
                LastChangeDate = x.LastChangeDate,
                RestrictedSendLists = x.RestrictedSendLists.Select(y=>new BaseTemplateDocumentRestrictedSendLists() {
                    PositionId=y.PositionId,
                    AccessLevel = (EnumDocumentAccess)y.AccessLevelId
                }).ToList(),
                SendLists = x.SendLists.Select(y => new BaseTemplateDocumentSendLists()
                {
                    SendType = (EnumSendType)y.SendTypeId,
                    TargetPositionId = y.TargetPositionId,
                    Description = y.Description,
                    DueDate = y.DueDate,
                    OrderNumber = y.OrderNumber,

                    DueDay = y.DueDay,
                    AccessLevel = (EnumDocumentAccess)y.AccessLevelId
                }).ToList()
            }).FirstOrDefault();
        }

        public int AddOrUpdateTemplate(IContext context, BaseTemplateDocument template)
        {
            // we should not implement it now
            //var dbContext = GetUserDmsContext(context);

            return 0;
        }
    }
}