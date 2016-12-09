using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.Enums;
using BL.Model.Exception;
using System.Linq;

namespace BL.Logic.DocumentCore.Commands
{
    public class ControlOnDocumentCommand: BaseDocumentCommand
    {
        private readonly IDocumentsDbProcess _documentDb;
        private readonly IDocumentOperationsDbProcess _operationDb;

        public ControlOnDocumentCommand(IDocumentsDbProcess documentDb, IDocumentOperationsDbProcess operationDb)
        {
            _documentDb = documentDb;
            _operationDb = operationDb;
        }

        private ControlOn Model
        {
            get
            {
                if (!(_param is ControlOn))
                {
                    throw new WrongParameterTypeError();
                }
                return (ControlOn) _param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            if (_document.Accesses != null && !_document.Accesses.Any(x => x.PositionId == positionId && x.IsInWork))
                return false;
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType);
            _document = _operationDb.AddNoteDocumentPrepare(_context, Model);
            //TODO проверка на контроль с одинаковыми задачами
            return true;
        }

        public override object Execute()
        {
            var taskId = CommonDocumentUtilities.GetDocumentTaskOrCreateNew(_context, _document, Model.Task);
            _document.Waits = CommonDocumentUtilities.GetNewDocumentWaits(_context, Model, EnumEventTypes.ControlOn, taskId, Model.IsAvailableWithinTask);
            _operationDb.AddDocumentWaits(_context, _document);
            return null;
        }

    }
}