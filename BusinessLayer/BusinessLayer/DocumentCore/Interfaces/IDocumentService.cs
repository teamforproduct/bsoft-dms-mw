using System.Collections.Generic;
using System.Threading.Tasks;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using BL.Model.DocumentCore.Actions;

namespace BL.Logic.DocumentCore.Interfaces
{
    public interface IDocumentService
    {
        void GetCountDocuments(IContext ctx, LicenceInfo licence);
        IEnumerable<FrontDocument> GetDocuments(IContext ctx, FilterDocument filters, UIPaging paging);
        Task<IEnumerable<FrontDocument>> GetDocumentsAsync(IContext ctx, FilterDocument filters, UIPaging paging);
        FrontDocument GetDocument(IContext ctx, int documentId, FilterDocumentById filter);
        IEnumerable<BaseSystemUIElement> GetModifyMetaData(IContext ctx, FrontDocument doc);
        object ExecuteAction(EnumDocumentActions act, IContext context, object param);
        FrontDocumentEvent GetDocumentEvent(IContext ctx, int eventId);
        IEnumerable<FrontDocumentEvent> GetDocumentEvents(IContext ctx, FilterDocumentEvent filter, UIPaging paging);
        IEnumerable<FrontDocumentWait> GetDocumentWaits(IContext ctx, FilterDocumentWait filter, UIPaging paging);
        IEnumerable<FrontDocumentSubscription> GetDocumentSubscriptions(IContext ctx, FilterDocumentSubscription filter, UIPaging paging);
        IEnumerable<FrontDocumentEvent> GetEventsForDocument(IContext ctx, int documentId, UIPaging paging);
        FrontRegistrationFullNumber GetNextRegisterDocumentNumber(IContext ctx, RegisterDocumentBase model);

        #region DocumentPapers
        FrontDocumentPaper GetDocumentPaper(IContext context, int itemId);

        IEnumerable<FrontDocumentPaper> GetDocumentPapers(IContext context, FilterDocumentPaper filter, UIPaging paging);

        #endregion DocumentPapers    
        #region DocumentPaperLists
        FrontDocumentPaperList GetDocumentPaperList(IContext context, int itemId);

        IEnumerable<FrontDocumentPaperList> GetDocumentPaperLists(IContext context, FilterDocumentPaperList filter);

        #endregion DocumentPaperLists        
    }
}