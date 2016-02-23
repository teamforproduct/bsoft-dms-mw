using System.Collections.Generic;
using System.Linq;
using BL.Logic.DependencyInjection;
using BL.Database.Dictionaries.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.InternalModel;

namespace BL.Logic.DocumentCore
{
    public class DocumentTagService : IDocumentTagService
    {
        #region System
        private readonly IDocumentTagsDbProcess _tagDb;

        public DocumentTagService(IDocumentTagsDbProcess tagDb)
        {
            _tagDb = tagDb;
        }
        #endregion System

        #region DocumentTags

        public IEnumerable<FrontDocumentTag> GetTags(IContext context, int documentId)
        {
            return _tagDb.GetTags(context, documentId).ToList();
        }

        public void ModifyDocumentTags(IContext context, ModifyDocumentTags model)
        {
            try
            {
                var item = new InternalDocumentTags
                {
                    DocumentId = model.DocumentId,
                    Tags = model.Tags,
                    LastChangeDate = DateTime.Now,
                    LastChangeUserId = context.CurrentAgentId,
                };
                _tagDb.ModifyDocumentTags(context, item);
            }
            catch (Exception ex)
            {
                throw new DatabaseError(ex);
            }
        }

        #endregion DocumentTags         
    }
}