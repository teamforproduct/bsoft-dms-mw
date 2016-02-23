using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using BL.Model.DictionaryCore.InternalModel;

namespace BL.Logic.DocumentCore.Interfaces
{
    public interface IDocumentService
    {
        IEnumerable<FrontDocument> GetDocuments(IContext ctx, FilterDocument filters, UIPaging paging);
        FrontDocument GetDocument(IContext ctx, int documentId);
        IEnumerable<BaseSystemUIElement> GetModifyMetaData(IContext cxt, FrontDocument doc);
        object ExecuteAction(EnumDocumentActions act, IContext context, object param);
        object ExecuteAdditionAction(EnumDocumentAdditionActions act, IContext context, object param);
    }
}