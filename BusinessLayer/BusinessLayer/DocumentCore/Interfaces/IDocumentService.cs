using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.Enums;
using BL.Model.SystemCore;

namespace BL.Logic.DocumentCore.Interfaces
{
    public interface IDocumentService
    {
        IEnumerable<FrontDocument> GetDocuments(IContext ctx, FilterDocument filters, UIPaging paging);
        FrontDocument GetDocument(IContext ctx, int documentId, FilterDocumentById filter);
        IEnumerable<BaseSystemUIElement> GetModifyMetaData(IContext cxt, FrontDocument doc);
        object ExecuteAction(EnumDocumentActions act, IContext context, object param);
        FrontDocumentEvent GetDocumentEvent(IContext ctx, int eventId);
        IEnumerable<FrontDocumentEvent> GetDocumentEvents(IContext ctx, FilterDocumentEvent filter, UIPaging paging);
        IEnumerable<FrontDocumentEvent> GetEventsForDocument(IContext ctx, int documentId, UIPaging paging);

        #region DocumentPapers
        FrontDocumentPaper GetDocumentPaper(IContext context, int itemId);

        IEnumerable<FrontDocumentPaper> GetDocumentPapers(IContext context, FilterDocumentPaper filter);

        #endregion DocumentPapers    
        #region DocumentPaperLists
        FrontDocumentPaperList GetDocumentPaperList(IContext context, int itemId);

        IEnumerable<FrontDocumentPaperList> GetDocumentPaperLists(IContext context, FilterDocumentPaperList filter);

        #endregion DocumentPaperLists        
    }
}