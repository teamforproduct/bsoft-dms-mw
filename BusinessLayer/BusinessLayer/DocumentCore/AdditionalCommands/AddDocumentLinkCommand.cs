using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Logic.Common;
using System.Linq;

namespace BL.Logic.DocumentCore.AdditionalCommands
{
    public class AddDocumentLinkCommand: BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        public AddDocumentLinkCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
        }

        private AddDocumentLink Model
        {
            get
            {
                if (!(_param is AddDocumentLink))
                {
                    throw new WrongParameterTypeError();
                }
                return (AddDocumentLink) _param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            if (_document.Accesses != null && !_document.Accesses.Any(x => x.PositionId == positionId && x.IsInWork))
                return false;
            if (_document.ExecutorPositionId != positionId
                )
            {
                return false;
            }

            return true;
        }

        public override bool CanExecute()
        {
            _document = _operationDb.AddDocumentLinkPrepare(_context, Model);
            if (_document?.Id == null || _document?.ParentDocumentId == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            if (_document.LinkId.HasValue && _document.ParentDocumentLinkId.HasValue && (_document.LinkId == _document.ParentDocumentLinkId))
            {
                throw new DocumentHasAlreadyHasLink();
            }
            _context.SetCurrentPosition(_document.ExecutorPositionId);
            _admin.VerifyAccess(_context, CommandType);
            if (!CanBeDisplayed(_context.CurrentPositionId))
            {
                throw new CouldNotPerformOperation();
            }
            return true;
        }

        public override object Execute()
        {
            CommonDocumentUtilities.SetLastChange(_context, _document);
            _document.Events = CommonDocumentUtilities.GetNewDocumentEvents(_context, _document.Id, EnumEventTypes.AddLink);
            _operationDb.AddDocumentLink(_context, _document);
            return null;
        }

    }
}