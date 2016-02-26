using System.Linq;
using BL.Database.Admins.Interfaces;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.Commands
{
    public class ControlChangeDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminsDbProcess _adminDb;

        private InternalDocumentWait _docWait;

        public ControlChangeDocumentCommand(IDocumentOperationsDbProcess operationDb, IAdminsDbProcess adminDb)
        {
            _operationDb = operationDb;
            _adminDb = adminDb;
        }

        private ControlChange Model
        {
            get
            {
                if (!(_param is ControlChange))
                {
                    throw new WrongParameterTypeError();
                }
                return (ControlChange)_param;
            }
        }

        public override bool CanBeDisplayed()
        {
            return true;
        }

        public override bool CanExecute()
        {
            _document = _operationDb.ControlOffDocumentPrepare(_context, Model.EventId);
            _docWait = _document.Waits.FirstOrDefault();
            if (_docWait?.OnEvent?.SourcePositionId == null || _docWait?.OnEvent?.SourcePositionId != _docWait?.OnEvent?.TargetPositionId)
            {
                throw new EventNotFoundOrUserHasNoAccess();
            }
            if (_docWait.OffEventId != null)
            {
                throw new WaitHasAlreadyClosed();
            }
            _context.SetCurrentPosition(_docWait.OnEvent.SourcePositionId);
            _adminDb.VerifyAccess(_context, CommandType);
            return true;
        }

        public override object Execute()
        {
            _docWait.OffEvent = CommonDocumentUtilities.GetNewDocumentEvent(_context, _docWait.DocumentId, EnumEventTypes.ControlChange,
                                    $"{_docWait.Task} / {Model.Description}", _docWait.OnEvent.TargetPositionId)
                                    .FirstOrDefault();
            CommonDocumentUtilities.SetLastChange(_context, _docWait);
            var controlOn = new ControlOn(Model, _docWait.DocumentId, _docWait.Task);
            var waits = CommonDocumentUtilities.GetNewDocumentWait(_context, controlOn).ToList();
            waits.First().ParentId = _docWait.Id;
            waits.Add(_docWait);
            _operationDb.ChangeDocumentWait(_context, waits);
            return _docWait.DocumentId;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.ControlChange;
    }
}