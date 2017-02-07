using System.Collections.Generic;
using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore.FrontModel;
using BL.CrossCutting.Interfaces;

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

        #endregion DocumentTags         
    }
}