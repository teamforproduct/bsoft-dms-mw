using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.PaperCommands
{
    public class MarkСorruptionDocumentPaperCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        public MarkСorruptionDocumentPaperCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
        }

        private EventPaper Model
        {
            get
            {
                if (!(_param is EventPaper))
                {
                    throw new WrongParameterTypeError();
                }
                return (EventPaper)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _document = _operationDb.EventDocumentPaperPrepare(_context, Model.Id);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            _admin.VerifyAccess(_context, CommandType);
            if (!CanBeDisplayed(_context.CurrentPositionId))
            {
                throw new CouldNotPerformThisOperation();
            }
            return true;
        }

        public override object Execute()
        {
            var paper = _document.Papers.First();

            paper.LastPaperEvent = CommonDocumentUtilities.GetNewDocumentPaperEvent(_context, paper.Id,
                EnumEventTypes.MarkOwnerDocumentPaper, Model.Description);
            CommonDocumentUtilities.SetLastChange(_context, paper);
            _operationDb.MarkOwnerDocumentPaper(_context, paper);
            return null;
        }

    }
}