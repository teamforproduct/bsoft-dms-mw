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

        private InternalDocumentPaper _paper;

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
            _paper = _document.Papers.First();
            _context.SetCurrentPosition(_document.ExecutorPositionId);
            _admin.VerifyAccess(_context, CommandType);
            if (!CanBeDisplayed(_context.CurrentPositionId))
            {
                throw new CouldNotPerformOperationWithPaper();
            }
            return true;
        }

        public override object Execute()
        {


            _paper.Name = Model.Name;
            _paper.Description = Model.Description;
            _paper.IsMain = Model.IsMain;
            _paper.IsOriginal = Model.IsOriginal;
            _paper.IsCopy = Model.IsCopy;
            _paper.PageQuantity = Model.PageQuantity;
            CommonDocumentUtilities.SetLastChange(_context, _paper);

            _operationDb.ModifyDocumentPaper(_context, _paper);
            return null;
        }

    }
}