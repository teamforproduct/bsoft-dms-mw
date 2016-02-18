using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;

namespace BL.Logic.DocumentCore.Interfaces
{
    public interface IDocumentTagService
    {
        #region DocumentTags
        BaseDocumentTag GetTag(IContext context, int id);

        IEnumerable<BaseDocumentTag> GetTags(IContext context, int documentId);

        #endregion DocumentTags     
    }
}