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

        public IEnumerable<FrontDocumentRestrictedSendList> GetRestrictedSendListBase(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                return CommonQueries.GetDocumentRestrictedSendList(dbContext, documentId);
            }
        }
        #endregion DocumentRestrictedSendLists

        #region DocumentSendLists

        public IEnumerable<FrontDocumentSendList> GetSendListBase(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                return CommonQueries.GetDocumentSendList(dbContext, documentId);
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
        #endregion DocumentSendLists         
    }
}