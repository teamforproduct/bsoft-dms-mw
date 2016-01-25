using System;
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
                RegistrationJournalId = null,
                RegistrationNumberPrefix = "AA",
                RegistrationNumberSuffix = "SS",
                
            });
            dbContext.SaveChanges();
        }

        public void UpdateDocument(IContext ctx, BaseDocument document)
        {
            var dbContext = GetUserDmsContext(ctx);
        }
    }
}