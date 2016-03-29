using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.PaperCommands
{
    public class ModifyDocumentPaperCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        public ModifyDocumentPaperCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
        }

        private ModifyDocumentPapers Model
        {
            get
            {
                if (!(_param is ModifyDocumentPapers))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDocumentPapers)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            if (_document.ExecutorPositionId != positionId
                )
            {
                return false;
            }

            return true;
        }

        public override bool CanExecute()
        {
            _document = _operationDb.ChangeDocumentPaperPrepare(_context, Model.Id);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            _context.SetCurrentPosition(_document.ExecutorPositionId);
            _admin.VerifyAccess(_context, CommandType);
            if (!CanBeDisplayed(_context.CurrentPositionId))
            {
                throw new CouldNotChangeAttributeLaunchPlan();
            }
            return true;
        }

        public override object Execute()
        {
            var paper = _document.Papers.First();

            paper.Name = Model.Name;
            paper.Description = Model.Description;
            paper.IsMain = Model.IsMain;
            paper.IsOriginal = Model.IsOriginal;
            paper.IsCopy = Model.IsCopy;
            paper.PageQuantity = Model.PageQuantity;
            paper.OrderNumber = Model.OrderNumber;
            CommonDocumentUtilities.SetLastChange(_context, paper);

            _operationDb.ModifyDocumentPaper(_context, paper);
            return null;
        }

    }
}