using System.Collections.Generic;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.Enums;
using BL.Model.Exception;
using System.Linq;
using BL.Model.DocumentCore.InternalModel;

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
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType);
            _document = _operationDb.AddNoteDocumentPrepare(_context, Model);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            return true;
        }

        public override object Execute()
        {
            var taskId = CommonDocumentUtilities.GetDocumentTaskOrCreateNew(_context, _document, Model.Task);
            var evtToAdd = new List<InternalDocumentEvent>();
            if (Model?.Positions.Count == 0)
            {
                throw new NobodyIsChosen();
            }
            var accList = _operationDb.GetDocumentAccesses(_context, Model.DocumentId);
            var actuelPosList = Model.Positions.Where(x => accList.Select(s => s.PositionId).Contains(x)).ToList();
            if (!actuelPosList.Any()) return null;

            var posInfos = _operationDb.GetInternalPositionsInfo(_context, actuelPosList);

            var description = Model.Description + (
                Model.IsAddPositionsInfo
                    ? "[" + string.Join(", ", posInfos.Select(x => x.PositionName)) + "]"
                    : "");
            evtToAdd.AddRange(actuelPosList.Select(targetPositionId => 
                CommonDocumentUtilities.GetNewDocumentEvent(_context, Model.DocumentId, EnumEventTypes.SendMessage, Model.EventDate, description, null, taskId, Model.IsAvailableWithinTask,targetPositionId)));
            _document.Events = evtToAdd;
            _operationDb.AddDocumentEvents(_context, _document);
            return null;
        }

    }
}