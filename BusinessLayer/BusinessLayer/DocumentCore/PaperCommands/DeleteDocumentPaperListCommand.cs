using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.PaperCommands
{
    public class DeleteDocumentPaperListCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        InternalDocumentPaperList _paperList;

        public DeleteDocumentPaperListCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
        }

        private int Model
        {
            get
            {
                if (!(_param is int))
                {
                    throw new WrongParameterTypeError();
                }
                return (int)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _paperList = _operationDb.DeleteDocumentPaperListPrepare(_context, Model);
            if (!_paperList.Events.Any(x=>x.PaperPlanDate.HasValue && !x.PaperSendDate.HasValue && !x.PaperRecieveDate.HasValue))
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            if (_paperList.Events.Any(x => !x.PaperPlanDate.HasValue || x.PaperSendDate.HasValue || x.PaperRecieveDate.HasValue))
            {
                throw new CouldNotPerformOperationWithPaper();
            }
            _context.SetCurrentPosition(_paperList.Events.First().SourcePositionId.Value);
            _admin.VerifyAccess(_context, CommandType);

            return true;
        }

        public override object Execute()
        {
            CommonDocumentUtilities.SetLastChange(_context, _paperList);
            _operationDb.DeleteDocumentPaperList(_context, _paperList);
            return null;
        }

    }
}