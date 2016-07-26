﻿using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.DependencyInjection;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Logic.SystemServices.AutoPlan;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Logic.DocumentCore.Interfaces;
using System.Transactions;

namespace BL.Logic.DocumentCore.SendListCommands
{
    public class AddDocumentSendListCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        private InternalDocumentSendList _sendList;

        public AddDocumentSendListCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
        }

        private ModifyDocumentSendList Model
        {
            get
            {
                if (!(_param is ModifyDocumentSendList))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDocumentSendList)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            if (CommandType == EnumDocumentActions.CopyDocumentSendList)
            {
                _actionRecords =
                    _document.SendLists.Select(x => new InternalActionRecord
                    {
                        SendListId = x.Id,
                    });
                if (!_actionRecords.Any() /*|| _document.IsLaunchPlan*/)
                {
                    return false;
                }
            }

            return true;
        }

        public override bool CanExecute()
        {

            _admin.VerifyAccess(_context, CommandType);
            _document = _operationDb.ChangeDocumentSendListPrepare(_context, Model.DocumentId, Model.Task);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            if (Model.IsInitial && _document.ExecutorPositionId != Model.CurrentPositionId)
            {
                throw new CouldNotPerformOperation();
            }
            //Model.IsInitial = !_document.IsLaunchPlan;
            var taskId = CommonDocumentUtilities.GetDocumentTaskOrCreateNew(_context, _document, Model.Task); //TODO исправление от кого????
            _sendList = CommonDocumentUtilities.GetNewDocumentSendList(_context, Model, taskId);
            var sendLists = _document.SendLists.ToList();
            sendLists.Add(_sendList);
            _document.SendLists = sendLists;

            CommonDocumentUtilities.VerifySendLists(_document);

            return true;
        }

        public override object Execute()
        {
            int res;
            var paperEvents = new List<InternalDocumentEvent>();
            if (Model.PaperEvents?.Any() ?? false)
                paperEvents.AddRange(Model.PaperEvents.Select(model => CommonDocumentUtilities.GetNewDocumentPaperEvent(_context, Model.DocumentId, model.Id, EnumEventTypes.MoveDocumentPaper, model.Description, _sendList.TargetPositionId, _sendList.TargetAgentId, _sendList.SourcePositionId, _sendList.SourceAgentId, false, false)));
            //            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                res = _operationDb.AddDocumentSendList(_context, new List<InternalDocumentSendList> { _sendList }, _document.Tasks, paperEvents).FirstOrDefault();
                if (Model.IsLaunchItem ?? false)
                {
                    var aplan = DmsResolver.Current.Get<IAutoPlanService>();
                    aplan.ManualRunAutoPlan(_context, res, _document.Id);
                }
                else
                {
                    var aplan = DmsResolver.Current.Get<IAutoPlanService>();
                    aplan.ManualRunAutoPlan(_context, null, _document.Id);
                }
                //                transaction.Complete();
            }
            return res;
        }

    }
}