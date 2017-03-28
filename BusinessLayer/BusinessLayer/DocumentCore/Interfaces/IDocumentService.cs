using System.Collections.Generic;
using System.Threading.Tasks;
using BL.CrossCutting.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using BL.Model.DocumentCore.Actions;
using BL.Model.FullTextSearch;

namespace BL.Logic.DocumentCore.Interfaces
{
    public interface IDocumentService
    {
        void GetCountDocuments(IContext ctx, LicenceInfo licence);
        IEnumerable<FrontDocument> GetDocuments(IContext ctx, FilterBase filters, UIPaging paging, EnumGroupCountType? groupCountType = null);
        FrontDocument GetDocument(IContext ctx, int documentId);

        IEnumerable<int> GetLinkedDocumentIds(IContext ctx, int documentId);

        IEnumerable<BaseSystemUIElement> GetModifyMetaData(IContext ctx, FrontDocument doc);
        object ExecuteAction(EnumDocumentActions act, IContext context, object param);
        FrontDocumentEvent GetDocumentEvent(IContext ctx, int eventId);
        IEnumerable<FrontDocumentEvent> GetDocumentEvents(IContext ctx, FilterBase filter, UIPaging paging);
        IEnumerable<FrontDocumentWait> GetDocumentWaits(IContext ctx, FilterBase filter, UIPaging paging);
        IEnumerable<FrontDocumentSubscription> GetDocumentSubscriptions(IContext ctx, FilterDocumentSubscription filter, UIPaging paging);
        IEnumerable<FrontDictionaryPosition> GetDocumentWorkGroup(IContext ctx, FilterDictionaryPosition filter,UIPaging paging);
        FrontRegistrationFullNumber GetNextRegisterDocumentNumber(IContext ctx, RegisterDocumentBase model);

        #region DocumentPapers
        FrontDocumentPaper GetDocumentPaper(IContext context, int itemId);

        IEnumerable<FrontDocumentPaper> GetDocumentPapers(IContext context, FilterDocumentPaper filter, UIPaging paging);

        #endregion DocumentPapers    
        #region DocumentPaperLists
        FrontDocumentPaperList GetDocumentPaperList(IContext context, int itemId);

        IEnumerable<FrontDocumentPaperList> GetDocumentPaperLists(IContext context, FilterDocumentPaperList filter, UIPaging paging);

        IEnumerable<FrontDocumentPaperList> GetMainDocumentPaperLists(IContext context, FullTextSearch ftSearch, FilterDocumentPaperList filter, UIPaging paging);

        #endregion DocumentPaperLists        

        #region DocumentAccesses
        IEnumerable<FrontDocumentAccess> GetDocumentAccesses(IContext ctx, FilterDocumentAccess filters, UIPaging paging);
        void CheckIsInWorkForControls(IContext ctx, FilterDocumentAccess filter);
        #endregion DocumentAccess
    }
}