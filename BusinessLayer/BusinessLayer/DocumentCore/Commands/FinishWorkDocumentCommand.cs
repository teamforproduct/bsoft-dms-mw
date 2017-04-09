using System.Linq;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.Commands
{
    public class FinishWorkDocumentCommand: BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        private InternalDocumentAccess _docAccess;

        public FinishWorkDocumentCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
        }

        private ChangeWorkStatus Model
        {
            get
            {
                if (!(_param is ChangeWorkStatus))
                {
                    throw new WrongParameterTypeError();
                }
                return (ChangeWorkStatus) _param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            if (!_document.Accesses.Any(x => x.PositionId == positionId && x.IsInWork && ((x.CountWaits ?? 0) == 0))
                )
            {
                return false;
            }

            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType);
            _document = _operationDb.ChangeIsInWorkAccessPrepare(_context, Model);
            _docAccess = _document?.Accesses.FirstOrDefault(x=>x.PositionId.HasValue);
            if (_docAccess == null
                || !CanBeDisplayed(_docAccess.PositionId.Value)
                )
            {
                throw new CouldNotPerformOperation();
            }
            return true;
        }

        public override object Execute()
        {
            _docAccess.IsInWork = false;
            CommonDocumentUtilities.SetLastChange(_context, _docAccess);
            _document.Events = CommonDocumentUtilities.GetNewDocumentEvents(_context, (int)EnumEntytiTypes.Document, Model.DocumentId, EnumEventTypes.SetOutWork, Model.EventDate, 
                                                                            Model.Description, null, null, Model.CurrentPositionId);
             _operationDb.ChangeIsInWorkAccess(_context, _document);
            return null;
        }

    }
}