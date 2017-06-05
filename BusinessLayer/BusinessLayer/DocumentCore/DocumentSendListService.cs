using BL.CrossCutting.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.Common;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using System.Collections.Generic;
using System.Linq;

namespace BL.Logic.DocumentCore
{
    public class DocumentSendListService : IDocumentSendListService
    {
        private readonly IDocumentSendListsDbProcess _documentDb;

        public DocumentSendListService(IDocumentSendListsDbProcess documentDb)
        {
            _documentDb = documentDb;
        }

        #region DocumentRestrictedSendLists

        public FrontDocumentRestrictedSendList GetRestrictedSendList(IContext context, int restrictedSendListId)
        {
            return _documentDb.GetRestrictedSendList(context, restrictedSendListId);
        }

        public int GetRestrictedSendListsCounter(IContext context, int documentId)
        {
            return _documentDb.GetRestrictedSendListsCounter(context, documentId);
        }
        public IEnumerable<FrontDocumentRestrictedSendList> GetRestrictedSendLists(IContext context, int documentId)
        {
            return _documentDb.GetRestrictedSendLists(context, documentId);
        }

        public IEnumerable<AutocompleteItem> GetRestrictedSendListsForAutocomplete(IContext context, int documentId)
        {
            return _documentDb.GetRestrictedSendListsForAutocomplete(context, documentId);
        }
        #endregion DocumentRestrictedSendLists

        #region DocumentSendLists
        public IEnumerable<FrontDocumentSendListStage> GetSendListStage(IContext context, int documentId, bool isLastStage = false)
        {

            var sendLists = _documentDb.GetSendLists(context, documentId).ToList();

            if (isLastStage)
            {
                int lastStage = sendLists.Count > 0 ? sendLists.Max(x => x.Stage??0) + 1 : 0;
                sendLists.Add(new FrontDocumentSendList { Id = 0, Stage = lastStage });
            }

            return CommonDocumentUtilities.GetSendListStage(sendLists);
        }

        public FrontDocumentSendList GetSendList(IContext context, int sendListId)
        {
            return _documentDb.GetSendList(context, sendListId);
        }

        public IEnumerable<FrontDocumentSendList> GetSendLists(IContext context, int documentId)
        {
            return _documentDb.GetSendLists(context, documentId).ToList();
        }
        public int GetSendListsCounter(IContext context, int documentId)
        {
            return _documentDb.GetSendListsCounter(context, documentId);
        }
        public IEnumerable<FrontDocument> GetAdditinalLinkedDocumentSendLists(IContext context, AdditinalLinkedDocumentSendList model)
        {
            var data = _documentDb.GetAdditinalLinkedDocumentSendListsPrepare(context, model);
            var sendListQry = data.Documents.Select(x => x.Id).Join(data.Positions, x => true, y => true, (x, y) => new { DocumentId = x, Position = y })
                .Where(x => !data.Accesses.Any(z => z.DocumentId == x.DocumentId && z.PositionId == x.Position.Id));
             var sendList = sendListQry.Select(x => new FrontDocumentSendList
                {
                    DocumentId = x.DocumentId,
                    SendType = EnumSendTypes.SendForInformation,
                    SendTypeCode = EnumSendTypes.SendForInformation.ToString(),
                    SendTypeName = data.SendTypeName,
                    AccessLevel = EnumAccessLevels.PersonallyAndIOAndReferents,
                    AccessGroups = new List<FrontDocumentSendListAccessGroup> { new FrontDocumentSendListAccessGroup
                    {
                        AccessGroupType = EnumEventAccessGroupTypes.Position,
                        AccessType = EnumEventAccessTypes.Target,
                        PositionId = x.Position.Id,
                        Name = x.Position.Name,
                        Details = new List<string> { x.Position.ExecutorAgentName },

                    } }
                }).ToList();
            data.Documents.ToList().ForEach(x=>x.SendLists = sendList.Where(y=>y.DocumentId == x.Id).ToList());
            var res = data.Documents.Where(x => x.SendLists.Any()).ToList();
            return res;
        }
        #endregion DocumentSendLists         
    }
}