using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Database.Common;
using BL.Database.DatabaseContext;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.Enums;

namespace BL.Database.Documents
{
    public class DocumentSendListsDbProcess : IDocumentSendListsDbProcess
    {
        public DocumentSendListsDbProcess()
        {
        }
        #region DocumentRestrictedSendLists
        public FrontDocumentRestrictedSendList GetRestrictedSendListBaseById(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx))
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
                    }).FirstOrDefault();

                return sendList;
            }
        }

        public IEnumerable<FrontDocumentRestrictedSendList> GetRestrictedSendListBase(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return CommonQueries.GetDocumentRestrictedSendList(dbContext, documentId);
            }
        }
        #endregion DocumentRestrictedSendLists

        #region DocumentSendLists

        public IEnumerable<FrontDocumentSendList> GetSendListBase(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return CommonQueries.GetDocumentSendList(dbContext, documentId);
            }
        }

        public FrontDocumentSendList GetSendListBaseById(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx))
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
                        TargetAgentName = x.TargetPosition.ExecutorAgent.Name??x.TargetAgent.Name,
                        Task = x.Task.Task,
                        Description = x.Description,
                        DueDate = x.DueDate,
                        DueDay = x.DueDay,
                        AccessLevel = (EnumDocumentAccesses)x.AccessLevelId,
                        AccessLevelName = x.AccessLevel.Name,
                        IsInitial = x.IsInitial,
                        StartEventId = x.StartEventId,
                        CloseEventId = x.CloseEventId,
                    }).FirstOrDefault();

                return sendLists;
            }
        }
        #endregion DocumentSendLists         
    }
}