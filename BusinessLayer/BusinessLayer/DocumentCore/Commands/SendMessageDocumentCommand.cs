using System.Collections.Generic;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.Enums;
using BL.Model.Exception;
using System.Linq;
using BL.Model.DocumentCore.InternalModel;
using BL.CrossCutting.Helpers;
using BL.Model.AdminCore;

namespace BL.Logic.DocumentCore.Commands
{
    public class SendMessageDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentsDbProcess _documentDb;
        private readonly IDocumentOperationsDbProcess _operationDb;

        public SendMessageDocumentCommand(IDocumentsDbProcess documentDb, IDocumentOperationsDbProcess operationDb)
        {
            _documentDb = documentDb;
            _operationDb = operationDb;
        }

        private SendMessage Model
        {
            get
            {
                if (!(_param is SendMessage))
                {
                    throw new WrongParameterTypeError();
                }
                return (SendMessage)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            if ((_document.Accesses?.Count() ?? 0) != 0 && !_document.Accesses.Any(x => x.PositionId == positionId && x.IsInWork))
                return false;
            return true;
//            if (_document.AccessesCount > 1)                return true;            else                return false;
        }

        public override bool CanExecute()
        {
            _adminProc.VerifyAccess(_context, CommandType);
            _document = _operationDb.AddNoteDocumentPrepare(_context, Model);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            _operationDb.SetRestrictedSendListsPrepare(_context, _document);
            _operationDb.SetParentEventAccessesPrepare(_context, _document, Model.ParentEventId);
            return true;
        }

        public override object Execute()
        {
            var taskId = CommonDocumentUtilities.GetDocumentTaskOrCreateNew(_context, _document, Model.Task);
            var newEvent = CommonDocumentUtilities.GetNewDocumentEvent(_context, (int)EnumEntytiTypes.Document, Model.DocumentId, EnumEventTypes.SendMessage, Model.EventDate, Model.Description, null, Model.ParentEventId, taskId, Model.TargetAccessGroups);
            //if (!newEvent.Accesses.Any(x => x.AccessType != EnumEventAccessTypes.Source)) //TODO Need verify?
            //{
            //    throw new NobodyIsChosen();
            //}
            CommonDocumentUtilities.VerifyAndSetDocumentAccess(_context, _document, newEvent.Accesses);
            _document.Events = new List<InternalDocumentEvent> { newEvent };
            using (var transaction = Transactions.GetTransaction())
            {
                _operationDb.AddDocumentEvents(_context, _document);
                if (Model.AddDocumentFiles?.Any() ?? false)
                {
                    Model.AddDocumentFiles.ForEach(x => { x.DocumentId = _document.Id; x.EventId = _document.Events.Select(y => y.Id).First(); });
                    _documentProc.ExecuteAction(EnumDocumentActions.AddDocumentFile, _context, Model.AddDocumentFiles);
                }
                transaction.Complete();
            }
            return null;
        }

    }
}