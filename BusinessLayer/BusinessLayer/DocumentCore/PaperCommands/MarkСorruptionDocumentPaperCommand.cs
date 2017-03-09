using System.Collections.Generic;
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

        private InternalDocumentPaper _paper;

        public MarkСorruptionDocumentPaperCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
        }

        private PaperEvent Model
        {
            get
            {
                if (!(_param is PaperEvent))
                {
                    throw new WrongParameterTypeError();
                }
                return (PaperEvent)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            _actionRecords =
                _document.Papers.Where(
                    x => x.IsInWork &&
                        x.LastPaperEvent.TargetPositionId == positionId &&
                        x.LastPaperEvent.PaperRecieveDate.HasValue )
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
            _document = _operationDb.EventDocumentPaperPrepare(_context, new PaperList { PaperId = new List<int> { Model.Id } });
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            _paper = _document.Papers.First();
            if (_paper?.LastPaperEvent?.TargetPositionId == null
                || !CanBeDisplayed(_paper.LastPaperEvent.TargetPositionId.Value)
                )
            {
                throw new CouldNotPerformOperationWithPaper();
            }
            _context.SetCurrentPosition(_paper.LastPaperEvent.TargetPositionId);
            _admin.VerifyAccess(_context, CommandType);
            return true;
        }

        public override object Execute()
        {
            _paper.LastPaperEvent = CommonDocumentUtilities.GetNewDocumentPaperEvent(_context, (int)EnumEntytiTypes.Document, _document.Id, _paper.Id, EnumEventTypes.MarkСorruptionDocumentPaper, Model.Description);
            _paper.IsInWork = false;
            CommonDocumentUtilities.SetLastChange(_context, _paper);
            _operationDb.MarkСorruptionDocumentPaper(_context, _paper);
            return null;
        }

    }
}