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

        private ModifyDocumentPaper Model
        {
            get
            {
                if (!(_param is ModifyDocumentPaper))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDocumentPaper)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            _actionRecords =
                _document.Papers.Where(
                    x => x.IsInWork &&
                        x.LastPaperEvent.TargetPositionId == positionId &&
                        x.LastPaperEvent.PaperRecieveDate.HasValue &&
                        x.LastPaperEvent.EventType == EnumEventTypes.AddNewPaper
                        )
                        .Select(x => new InternalActionRecord
                        {
                            PaperId = x.Id,
                        });
            if (!_actionRecords.Any())
            {
                return false;
            }
            return true;
        }

        public override bool CanExecute()
        {
            _document = _operationDb.ModifyDocumentPaperPrepare(_context, Model.Id, Model);
            _paper = _document?.Papers.First();
            if (_paper == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            //if (_document.Papers.Count() > 1)
            //{
            //    throw new RecordNotUnique();
            //}
            if (!Model.IsCopy || !_paper.IsCopy)
            {
                _context.SetCurrentPosition(_document.ExecutorPositionId);
            }
            else
            {
                _context.SetCurrentPosition(_paper.LastPaperEvent.TargetPositionId);
            }
            _adminProc.VerifyAccess(_context, CommandType);
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