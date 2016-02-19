using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.InternalModel;

namespace BL.Database.Documents.Interfaces
{
    public interface IDocumentTagsDbProcess
    {
        #region DocumentTags
        IEnumerable<FrontDocumentTag> GetTags(IContext ctx, int documentId);
        void ModifyDocumentTags(IContext context, InternalDocumentTags model);
        #endregion DocumentTags         
    }
}