using System;
using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Database.Common;
using BL.Logic.Helpers;
using BL.Database.DatabaseContext;
using BL.Database.DBModel.Document;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;

namespace BL.Database.Documents
{
    public class DocumentSendListsDbProcess : IDocumentSendListsDbProcess
    {
        private readonly IConnectionStringHelper _helper;

        public DocumentSendListsDbProcess(IConnectionStringHelper helper)
        {
            _helper = helper;
        }
        #region DocumentRestrictedSendLists
        public ModifyDocumentRestrictedSendList GetRestrictedSendListById(IContext ctx, int id)
        {

            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {

                var sendList = dbContext.DocumentRestrictedSendListsSet
                    .Where(x => x.Id == id)
                    .Select(x => new ModifyDocumentRestrictedSendList
                    {
                        Id = x.Id,
                        DocumentId = x.DocumentId,
                        PositionId = x.PositionId,
                        AccessLevel = (EnumDocumentAccesses)x.AccessLevelId,
                    }).FirstOrDefault();

                return sendList;
            }
        }
        public FrontDocumentRestrictedSendList GetRestrictedSendListBaseById(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                //TODO: Refactoring
                var sendList = dbContext.DocumentRestrictedSendListsSet
                    .Where(x => x.Id == id)
                    .Select(x => new FrontDocumentRestrictedSendList
                    {
                        Id = x.Id,
                        DocumentId = x.DocumentId,
                        PositionId = x.PositionId,
                        PositionName = x.Position.Name,
                        PositionExecutorAgentName = x.Position.ExecutorAgent.Name,
                        AccessLevel = (EnumDocumentAccesses)x.AccessLevelId,
                        AccessLevelName = x.AccessLevel.Name,
                        LastChangeUserId = x.LastChangeUserId,
                        LastChangeDate = x.LastChangeDate,
                        GeneralInfo = string.Empty
                    }).FirstOrDefault();

                return sendList;
            }
        }
        public IEnumerable<ModifyDocumentRestrictedSendList> GetRestrictedSendList(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {

                var sendLists = dbContext.DocumentRestrictedSendListsSet
                    .Where(x => x.DocumentId == documentId)
                    .Select(x => new ModifyDocumentRestrictedSendList
                    {
                        Id = x.Id,
                        DocumentId = x.DocumentId,
                        PositionId = x.PositionId,
                        AccessLevel = (EnumDocumentAccesses)x.AccessLevelId,
                    }).ToList();

                return sendLists;
            }
        }

        public IEnumerable<FrontDocumentRestrictedSendList> GetRestrictedSendListBase(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                return CommonQueries.GetDocumentRestrictedSendList(dbContext, documentId);
            }
        }

        public void UpdateRestrictedSendList(IContext ctx, ModifyDocumentRestrictedSendList restrictedSendList)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {

                var sendList =
                    dbContext.DocumentRestrictedSendListsSet.FirstOrDefault(x => x.Id == restrictedSendList.Id);
                if (sendList?.Id > 0)
                {
                    sendList.DocumentId = restrictedSendList.DocumentId;
                    sendList.PositionId = restrictedSendList.PositionId;
                    sendList.AccessLevelId = (int)restrictedSendList.AccessLevel;
                    sendList.LastChangeUserId = ctx.CurrentAgentId;
                    sendList.LastChangeDate = DateTime.Now;

                    dbContext.SaveChanges();
                }
            }
        }

        public IEnumerable<int> AddRestrictedSendList(IContext ctx, IEnumerable<ModifyDocumentRestrictedSendList> restrictedSendLists)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {

                var sendLists = restrictedSendLists.Select(x => new DocumentRestrictedSendLists
                {
                    DocumentId = x.DocumentId,
                    PositionId = x.PositionId,
                    AccessLevelId = (int)x.AccessLevel,
                    LastChangeUserId = ctx.CurrentAgentId,
                    LastChangeDate = DateTime.Now
                }).ToList();

                dbContext.DocumentRestrictedSendListsSet.AddRange(sendLists);

                dbContext.SaveChanges();

                return sendLists.Select(x => x.Id).ToList();
            }
        }

        public void DeleteRestrictedSendList(IContext ctx, int restrictedSendListId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var sendList = dbContext.DocumentRestrictedSendListsSet.FirstOrDefault(x => x.Id == restrictedSendListId);
                if (sendList != null)
                {
                    dbContext.DocumentRestrictedSendListsSet.Remove(sendList);
                    dbContext.SaveChanges();
                }
            }
        }
        #endregion DocumentRestrictedSendLists

        #region DocumentSendLists
        public IEnumerable<ModifyDocumentSendList> GetSendList(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {

                var sendLists = dbContext.DocumentSendListsSet
                    .Where(x => x.DocumentId == documentId)
                    .Select(x => new ModifyDocumentSendList
                    {
                        Id = x.Id,
                        DocumentId = x.DocumentId,
                        Stage = x.Stage,
                        SendType = (EnumSendTypes)x.SendTypeId,
                        TargetPositionId = x.TargetPositionId,
                        Description = x.Description,
                        DueDate = x.DueDate,
                        DueDay = x.DueDay,
                        AccessLevel = (EnumDocumentAccesses)x.AccessLevelId
                    }).ToList();

                return sendLists;
            }
        }

        public IEnumerable<FrontDocumentSendList> GetSendListBase(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                return CommonQueries.GetDocumentSendList(dbContext, documentId);
            }
        }

        public ModifyDocumentSendList GetSendListById(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {

                var sendLists = dbContext.DocumentSendListsSet
                    .Where(x => x.Id == id)
                    .Select(x => new ModifyDocumentSendList
                    {
                        Id = x.Id,
                        DocumentId = x.DocumentId,
                        Stage = x.Stage,
                        SendType = (EnumSendTypes)x.SendTypeId,
                        TargetPositionId = x.TargetPositionId,
                        Description = x.Description,
                        DueDate = x.DueDate,
                        DueDay = x.DueDay,
                        AccessLevel = (EnumDocumentAccesses)x.AccessLevelId
                    }).FirstOrDefault();

                return sendLists;
            }
        }

        public FrontDocumentSendList GetSendListBaseById(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                //TODO: Refactoring
                var sendLists = dbContext.DocumentSendListsSet
                    .Where(x => x.Id == id)
                    .Select(x => new FrontDocumentSendList
                    {
                        Id = x.Id,
                        DocumentId = x.DocumentId,
                        Stage = x.Stage,
                        SendType = (EnumSendTypes)x.SendTypeId,
                        SendTypeName = x.SendType.Name,
                        SendTypeCode = x.SendType.Code,
                        SendTypeIsImportant = x.SendType.IsImportant,
                        TargetPositionId = x.TargetPositionId,
                        TargetPositionName = x.TargetPosition.Name,
                        TargetPositionExecutorAgentName = x.TargetPosition.ExecutorAgent.Name,
                        Description = x.Description,
                        DueDate = x.DueDate,
                        DueDay = x.DueDay,
                        AccessLevel = (EnumDocumentAccesses)x.AccessLevelId,
                        AccessLevelName = x.AccessLevel.Name,
                        IsInitial = x.IsInitial,
                        StartEventId = x.StartEventId,
                        CloseEventId = x.CloseEventId,
                        LastChangeUserId = x.LastChangeUserId,
                        LastChangeDate = x.LastChangeDate,
                        GeneralInfo = string.Empty
                    }).FirstOrDefault();

                return sendLists;
            }
        }

        public void UpdateSendList(IContext ctx, ModifyDocumentSendList sendList)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {

                var sl = dbContext.DocumentSendListsSet.FirstOrDefault(x => x.Id == sendList.Id);
                if (sl?.Id > 0)
                {
                    sl.DocumentId = sendList.DocumentId;
                    sl.Stage = sendList.Stage;
                    sl.SendTypeId = (int)sendList.SendType;
                    sl.TargetPositionId = sendList.TargetPositionId;
                    sl.Description = sendList.Description;
                    sl.DueDate = sendList.DueDate;
                    sl.DueDay = sendList.DueDay;
                    sl.AccessLevelId = (int)sendList.AccessLevel;
                    sl.IsInitial = true;
                    sl.StartEventId = null;
                    sl.CloseEventId = null;
                    sl.LastChangeUserId = ctx.CurrentAgentId;
                    sl.LastChangeDate = DateTime.Now;
                    dbContext.SaveChanges();
                }
            }
        }

        public IEnumerable<int> AddSendList(IContext ctx, IEnumerable<ModifyDocumentSendList> sendLists)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {

                var sls = sendLists.Select(x => new DocumentSendLists
                {
                    DocumentId = x.DocumentId,
                    Stage = x.Stage,
                    SendTypeId = (int)x.SendType,
                    TargetPositionId = x.TargetPositionId,
                    Description = x.Description,
                    DueDate = x.DueDate,
                    DueDay = x.DueDay,
                    AccessLevelId = (int)x.AccessLevel,
                    IsInitial = true,
                    StartEventId = null,
                    CloseEventId = null,
                    LastChangeUserId = ctx.CurrentAgentId,
                    LastChangeDate = DateTime.Now
                }).ToList();

                dbContext.DocumentSendListsSet.AddRange(sls);
                dbContext.SaveChanges();

                return sls.Select(x => x.Id).ToList();
            }
        }

        public void DeleteSendList(IContext ctx, int sendListId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var sendList = dbContext.DocumentSendListsSet.FirstOrDefault(x => x.Id == sendListId);
                if (sendList != null)
                {
                    dbContext.DocumentSendListsSet.Remove(sendList);
                    dbContext.SaveChanges();
                }
            }
        }
        #endregion DocumentSendLists         
    }
}