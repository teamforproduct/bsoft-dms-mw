using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Logic.Common;
using System.Linq;
using BL.Model.DocumentCore.InternalModel;
using System.Collections.Generic;

namespace BL.Logic.DocumentCore.AdditionalCommands
{
    public class AddDocumentLinkCommand : BaseDocumentCommand
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
                return (AddDocumentLink)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            if ((_document.Accesses?.Count() ?? 0) != 0 && !_document.Accesses.Any(x => x.PositionId == positionId && x.IsInWork))
                return false;
            //if (_document.ExecutorPositionId != positionId
            //    )
            //{
            //    return false;
            //}

            return true;
        }

        public override bool CanExecute()
        {
            if (Model.DocumentId == Model.ParentDocumentId)
            {
                throw new CouldNotPerformOperation();
            }
            _admin.VerifyAccess(_context, CommandType);
            _document = _operationDb.AddDocumentLinkPrepare(_context, Model);
            if (_document?.Id == null || _document?.ParentDocumentId == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            if (_document.LinkId.HasValue && _document.ParentDocumentLinkId.HasValue && (_document.LinkId == _document.ParentDocumentLinkId))
            {
                throw new DocumentHasAlreadyHasLink();
            }

            if (!CanBeDisplayed(_context.CurrentPositionId))
            {
                throw new CouldNotPerformOperation();
            }
            return true;
        }

        public override object Execute()
        {
            CommonDocumentUtilities.SetLastChange(_context, _document);
            var newEvent = CommonDocumentUtilities.GetNewDocumentEvent(_context, (int)EnumEntytiTypes.Document, _document.Id, EnumEventTypes.AddLink);
            if (!newEvent.SourcePositionExecutorAgentId.HasValue)
            {
                throw new ExecutorAgentForPositionIsNotDefined();
            }
            _document.ExecutorPositionId = newEvent.SourcePositionId ?? (int)EnumSystemPositions.AdminPosition;
            _document.ExecutorPositionExecutorAgentId = newEvent.SourcePositionExecutorAgentId.Value;
            _document.ExecutorPositionExecutorTypeId = newEvent.SourcePositionExecutorTypeId;
            _document.Events = new List<InternalDocumentEvent> { newEvent };
            _operationDb.AddDocumentLink(_context, _document);
            return null;
        }

    }
}