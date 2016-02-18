using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;

namespace BL.Database.Documents.Interfaces
{
    public interface IDocumentTagsDbProcess
    {
        IEnumerable<BaseDocumentTag> GetTags(IContext ctx, int documentId);

        BaseDocumentTag GetTag(IContext ctx, int id);
    }
}