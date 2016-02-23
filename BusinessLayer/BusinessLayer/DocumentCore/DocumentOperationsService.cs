using System;
using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.Dictionaries.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Database.SystemDb;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore;
using BL.Database.Admins.Interfaces;
using BL.Model.AdminCore;
using BL.Model.DictionaryCore;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.DocumentCore.InternalModel;

namespace BL.Logic.DocumentCore
{
    public class DocumentOperationsService : IDocumentOperationsService
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminsDbProcess _adminDb;

        public DocumentOperationsService(IDocumentOperationsDbProcess operationDb, IAdminsDbProcess adminDb)
        {

            _operationDb = operationDb;
        }

        #region Operation with document
        public IEnumerable<InternalDictionaryPositionWithActions> GetDocumentActions(IContext ctx, int documentId)
        {

            var document = _operationDb.GetDocumentActionsPrepare(ctx, documentId);
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            var positions = dictDb.GetDictionaryPositionsWithActions(ctx, new FilterDictionaryPosition { Id = ctx.CurrentPositionsIdList });
            var systemDb = DmsResolver.Current.Get<ISystemDbProcess>();
            foreach (var position in positions)
            {
                position.Actions = systemDb.GetSystemActions(ctx, new FilterSystemAction() { Object = EnumObjects.Documents, IsAvailable = true, PositionsIdList = new List<int> { position.Id } });
                if (document.IsRegistered || position.Id != document.ExecutorPositionId)
                {
                    position.Actions = position.Actions.Where(x => x.DocumentAction != EnumDocumentActions.ModifyDocument).ToList();
                    position.Actions = position.Actions.Where(x => x.DocumentAction != EnumDocumentActions.DeleteDocument).ToList();
                }
                if (document.IsRegistered)
                {
                    position.Actions = position.Actions.Where(x => x.DocumentAction != EnumDocumentActions.RegisterDocument).ToList();
                    position.Actions = position.Actions.Where(x => x.DocumentAction != EnumDocumentActions.ChangeExecutor).ToList();
                }
                position.Actions.Where(x => x.DocumentAction == EnumDocumentActions.ControlOff).ToList()
                    .ForEach(x =>
                    {
                        x.ActionRecords = new List<InternalActionRecord>()
                        {
                            new InternalActionRecord() {Id = 1, Description = "TEST1"},
                            new InternalActionRecord() {Id = 2, Description = "TEST2"}
                        };
                    });
            }

            return positions;//actions;
        }


        #endregion         
    }
}