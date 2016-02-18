﻿using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.Dictionaries.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.Exception;

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
        public BaseDocumentTag GetTag(IContext context, int id)
        {
            return _tagDb.GetTag(context, id);
        }

        public IEnumerable<BaseDocumentTag> GetTags(IContext context, int documentId)
        {
            return _tagDb.GetTags(context, documentId).ToList();
        }

        #endregion DocumentTags         
    }
}