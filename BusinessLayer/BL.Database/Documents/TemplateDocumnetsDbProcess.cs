using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;
using System.Linq;
using BL.Database.Documents.Interfaces;

namespace BL.Database.Documents
{
    public class TemplateDocumnetsDbProcess : CoreDb.CoreDb, ITemplateDocumnetsDbProcess
    {
        public IEnumerable<BaseTemplateDocument> GetTemplateDocument(IContext context)
        {
            var dbContext = GetUserDmsContext(context);
            return dbContext.TemplateDocumentsSet.Select(x => new BaseTemplateDocument
            {
                Id = x.Id,
                DocumentDirectionId = x.DocumentDirectionId,
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
                DocumentDirectionId = x.DocumentDirectionId,
                DocumentTypeId = x.DocumentTypeId,
                Description = x.Description,
                DocumentSubjectId = x.DocumentSubjectId,
                RegistrationJournalId = x.RegistrationJournalId,
                RestrictedSendListId = x.RestrictedSendListId,
                LastChangeUserId = x.LastChangeUserId,
                LastChangeDate = x.LastChangeDate
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