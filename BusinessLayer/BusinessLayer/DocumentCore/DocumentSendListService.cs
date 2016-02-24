using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Logic.DependencyInjection;
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
            return _documentDb.GetRestrictedSendListBaseById(context, restrictedSendListId);
        }

        public IEnumerable<FrontDocumentRestrictedSendList> GetRestrictedSendLists(IContext context, int documentId)
        {
            return _documentDb.GetRestrictedSendListBase(context, documentId);
        }

        public void UpdateRestrictedSendList(IContext context, ModifyDocumentRestrictedSendList restrictedSendList)
        {
            var validRestrictedSendLists = _documentDb.GetRestrictedSendList(context, restrictedSendList.DocumentId).ToList();
            validRestrictedSendLists.Remove(validRestrictedSendLists.FirstOrDefault(x => x.Id == restrictedSendList.Id));
            validRestrictedSendLists.Add(restrictedSendList);
            ValidSendListsByRestrictedSendLists(context, restrictedSendList.DocumentId, validRestrictedSendLists);

            _documentDb.UpdateRestrictedSendList(context, restrictedSendList);
        }
        public int AddRestrictedSendList(IContext context, ModifyDocumentRestrictedSendList restrictedSendList)
        {
            var validRestrictedSendLists = _documentDb.GetRestrictedSendList(context, restrictedSendList.DocumentId).ToList();
            validRestrictedSendLists.Add(restrictedSendList);
            ValidSendListsByRestrictedSendLists(context, restrictedSendList.DocumentId, validRestrictedSendLists);

            return _documentDb.AddRestrictedSendList(context, new List<ModifyDocumentRestrictedSendList> { restrictedSendList }).FirstOrDefault();
        }

        public void AddRestrictedSendListByStandartSendLists(IContext context, ModifyDocumentRestrictedSendListByStandartSendList model)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            var dicStandSendList = dictDb.GetDictionaryStandartSendList(context, model.StandartSendListId);

            var restrictedSendLists = dicStandSendList.StandartSendListContents.Select(x => new ModifyDocumentRestrictedSendList
            {
                DocumentId = model.DocumentId,
                PositionId = x.TargetPositionId,
                AccessLevel = (EnumDocumentAccesses)x.AccessLevelId.GetValueOrDefault()
            });

            var validRestrictedSendLists = _documentDb.GetRestrictedSendList(context, model.DocumentId).ToList();
            validRestrictedSendLists.AddRange(restrictedSendLists);
            ValidSendListsByRestrictedSendLists(context, model.DocumentId, validRestrictedSendLists);

            _documentDb.AddRestrictedSendList(context, restrictedSendLists);
        }

        public void DeleteRestrictedSendList(IContext context, int restrictedSendListId)
        {
            var restrictedSendList = _documentDb.GetRestrictedSendListById(context, restrictedSendListId);

            var validRestrictedSendLists = _documentDb.GetRestrictedSendList(context, restrictedSendList.DocumentId).ToList();
            validRestrictedSendLists.Remove(validRestrictedSendLists.FirstOrDefault(x => x.Id == restrictedSendList.Id));
            ValidSendListsByRestrictedSendLists(context, restrictedSendList.DocumentId, validRestrictedSendLists);

            _documentDb.DeleteRestrictedSendList(context, restrictedSendListId);
        }
        #endregion DocumentRestrictedSendLists

        #region Valid SendLists
        // TODO CHECK IT
        public void ValidSendListsBySendLists(IContext context, int documentId, IEnumerable<ModifyDocumentSendList> sendLists)
        {
            var restrictedSendLists = _documentDb.GetRestrictedSendList(context, documentId);

            ValidSendLists(context, documentId, sendLists, restrictedSendLists);
        }

        public void ValidSendListsByRestrictedSendLists(IContext context, int documentId, IEnumerable<ModifyDocumentRestrictedSendList> restrictedSendLists)
        {
            var sendLists = _documentDb.GetSendList(context, documentId);

            ValidSendLists(context, documentId, sendLists, restrictedSendLists);
        }

        public void ValidSendLists(IContext context, int documentId, IEnumerable<ModifyDocumentSendList> sendLists, IEnumerable<ModifyDocumentRestrictedSendList> restrictedSendLists)
        {

            if (restrictedSendLists.GroupBy(x => new { x.DocumentId, x.PositionId }).Any(x => x.Count() > 1))
            {
                throw new DocumentRestrictedSendListDuplication();
            }

            if (sendLists.GroupBy(x => new { x.DocumentId, x.TargetPositionId, x.SendType }).Any(x => x.Count() > 1))
            {
                throw new DocumentSendListDuplication();
            }

            if (restrictedSendLists?.Count() > 0
                && sendLists.GroupJoin(restrictedSendLists
                    , sl => sl.TargetPositionId
                    , rsl => rsl.PositionId
                    , (sl, rsls) => new { sl, rsls }).Any(x => x.rsls.Count() == 0))
            {
                throw new DocumentSendListNotFoundInDocumentRestrictedSendList();
            }

            var templateDocumentDb = DmsResolver.Current.Get<ITemplateDocumentsDbProcess>();
            var templateDocument = templateDocumentDb.GetTemplateDocumentByDocumentId(context, documentId);

            if (templateDocument == null)
            {
                throw new TemplateDocumentNotFoundOrUserHasNoAccess();
            }

            if (templateDocument.IsHard)
            {
                if (templateDocument.RestrictedSendLists.Any()
                    && templateDocument.RestrictedSendLists.GroupJoin(restrictedSendLists
                        , trsl => trsl.PositionId
                        , rsl => rsl.PositionId
                        , (trsl, rsls) => new { trsl, rsls }).Any(x => x.rsls.Count() == 0))
                {
                    throw new DocumentRestrictedSendListDoesNotMatchTheTemplate();
                }

                if (templateDocument.SendLists.Any()
                    && templateDocument.SendLists.GroupJoin(sendLists
                        , trsl => new { trsl.TargetPositionId, trsl.SendType }
                        , rsl => new { rsl.TargetPositionId, rsl.SendType }
                        , (trsl, rsls) => new { trsl, rsls }).Any(x => x.rsls.Count() == 0))
                {
                    throw new DocumentSendListDoesNotMatchTheTemplate();
                }
            }
        }
        #endregion

        #region DocumentSendLists
        public IEnumerable<FrontDocumentSendListStage> GetSendListStage(IEnumerable<FrontDocumentSendList> sendLists)
        {
            if (sendLists?.Count() > 0)
            {
                return Enumerable.Range(0, sendLists.Max(x => x.Stage) + 1)
                    .GroupJoin(sendLists, s => s, sl => sl.Stage
                    , (s, sls) => new { s, sls = sls.Where(x => x.Id > 0) })
                    .Select(x => new FrontDocumentSendListStage
                    {
                        Stage = x.s,
                        SendLists = x.sls.ToList()
                    }).ToList();

            }
            else
                return new List<FrontDocumentSendListStage>();
        }
        public IEnumerable<FrontDocumentSendListStage> GetSendListStage(IContext context, int documentId, bool isLastStage = false)
        {

            var sendLists = _documentDb.GetSendListBase(context, documentId).ToList();

            if (isLastStage)
            {
                int lastStage = sendLists.Count > 0 ? sendLists.Max(x => x.Stage) + 1 : 0;
                sendLists.Add(new FrontDocumentSendList { Id = 0, Stage = lastStage });
            }

            return GetSendListStage(sendLists);
        }

        public FrontDocumentSendList GetSendList(IContext context, int sendListId)
        {
            return _documentDb.GetSendListBaseById(context, sendListId);
        }

        public IEnumerable<FrontDocumentSendList> GetSendLists(IContext context, int documentId)
        {
            return _documentDb.GetSendListBase(context, documentId).ToList();
        }

        public void UpdateSendList(IContext context, ModifyDocumentSendList sendList)
        {
            var validSendLists = _documentDb.GetSendList(context, sendList.DocumentId).ToList();
            validSendLists.Remove(validSendLists.FirstOrDefault(x => x.Id == sendList.Id));
            validSendLists.Add(sendList);
            ValidSendListsBySendLists(context, sendList.DocumentId, validSendLists);

            _documentDb.UpdateSendList(context, sendList);
        }
        public int AddSendList(IContext context, ModifyDocumentSendList sendList)
        {
            var validSendLists = _documentDb.GetSendList(context, sendList.DocumentId).ToList();
            validSendLists.Add(sendList);
            ValidSendListsBySendLists(context, sendList.DocumentId, validSendLists);

            return _documentDb.AddSendList(context, new List<ModifyDocumentSendList> { sendList }).FirstOrDefault();
        }

        public void AddSendListByStandartSendLists(IContext context, ModifyDocumentSendListByStandartSendList model)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            var dicStandSendList = dictDb.GetDictionaryStandartSendList(context, model.StandartSendListId);

            var sendLists = dicStandSendList.StandartSendListContents.Select(x => new ModifyDocumentSendList
            {
                DocumentId = model.DocumentId,
                Stage = x.Stage,
                SendType = (EnumSendTypes)x.SendTypeId,
                TargetPositionId = x.TargetPositionId,
                Description = x.Description,
                DueDate = x.DueDate,
                DueDay = x.DueDay.GetValueOrDefault(),
                AccessLevel = (EnumDocumentAccesses)x.AccessLevelId.GetValueOrDefault(),
            });

            var validSendLists = _documentDb.GetSendList(context, model.DocumentId).ToList();
            validSendLists.AddRange(sendLists);
            ValidSendListsBySendLists(context, model.DocumentId, validSendLists);

            _documentDb.AddSendList(context, sendLists);
        }

        public void DeleteSendList(IContext context, int sendListId)
        {
            var sendList = _documentDb.GetSendListById(context, sendListId);

            var validSendLists = _documentDb.GetSendList(context, sendList.DocumentId).ToList();
            validSendLists.Remove(validSendLists.FirstOrDefault(x => x.Id == sendList.Id));
            ValidSendListsBySendLists(context, sendList.DocumentId, validSendLists);

            _documentDb.DeleteSendList(context, sendListId);
        }

        public bool AddSendListStage(IContext context, ModifyDocumentSendListStage model)
        {
            if (model.Stage >= 0)
            {
                var sendLists = _documentDb.GetSendList(context, model.DocumentId);

                sendLists = sendLists.Where(x => x.Stage >= model.Stage);

                if (sendLists.Any())
                    foreach (var sendList in sendLists)
                    {
                        sendList.Stage++;
                        _documentDb.UpdateSendList(context, sendList);
                    }
                else
                    return true;
            }
            return false;
        }

        public void DeleteSendListStage(IContext context, ModifyDocumentSendListStage model)
        {
            var sendLists = _documentDb.GetSendList(context, model.DocumentId).ToList();

            ValidSendListsBySendLists(context, model.DocumentId, sendLists.Where(x => x.Stage != model.Stage));

            foreach (var sendList in sendLists.Where(x => x.Stage == model.Stage))
            {
                _documentDb.DeleteSendList(context, sendList.Id);
            }

            foreach (var sendList in sendLists.Where(x => x.Stage > model.Stage))
            {
                sendList.Stage--;
                _documentDb.UpdateSendList(context, sendList);
            }

        }

        #endregion DocumentSendLists         
    }
}