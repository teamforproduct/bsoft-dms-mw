using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Logic.Common;
using System.Linq;
using BL.Model.DocumentCore.InternalModel;

namespace BL.Logic.DocumentCore.AdditionalCommands
{
    public class DeleteDocumentLinkCommand: BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private InternalDocumentLink _link;

        public DeleteDocumentLinkCommand(IDocumentOperationsDbProcess operationDb)
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
                return (int) _param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            if ((_document.Accesses?.Count() ?? 0) != 0 && !_document.Accesses.Any(x => x.PositionId == positionId && x.IsInWork))
                return false;
            _actionRecords =
                _document.Links.Where(
                    x =>
                        x.ExecutorPositionId == positionId)
                        .Select(x => new InternalActionRecord
                        {
                            LinkId = x.Id
                        });
            if (!_actionRecords.Any())
            {
                return false;
            }

            return true;
        }

        public override bool CanExecute()
        {
            _document = _operationDb.DeleteDocumentLinkPrepare(_context, Model);
            _link = _document.Links.FirstOrDefault();
            if (_document?.Id == null || _document?.LinkId == null || _link == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            _context.SetCurrentPosition(_link.ExecutorPositionId);
            _adminProc.VerifyAccess(_context, CommandType);
            if (!CanBeDisplayed(_context.CurrentPositionId))
            {
                throw new CouldNotPerformOperation();
            }
            return true;
        }

        public override object Execute()
        {
            _document.OldLinkSet = _document.OldLinks.Select(x => x.DocumentId).Concat(_document.OldLinks.Select(x => x.ParentDocumentId)).GroupBy(x => x).Select(x => x.Key).ToList();
            _document.NewLinkSet = CommonDocumentUtilities.GetLinkedDocuments(_link.DocumentId, _document.OldLinks.Where(x=>x.Id!=_link.Id));
            _document.OldLinkSet = _document.OldLinkSet.Except(_document.NewLinkSet).ToList();
            if (_document.OldLinkSet.Count()<=1)
            {
                _document.OldLinkId = null;
            } 
            else if (!_document.OldLinkSet.ToList().Contains(_document.LinkId.Value))
            {
                _document.OldLinkId = _document.OldLinkSet.Min();
            }
            else
            {
                _document.OldLinkId = _document.LinkId.Value;
            }
            if (_document.NewLinkSet.Count() <= 1)
            {
                _document.NewLinkId = null;
            }
            else if (!_document.NewLinkSet.ToList().Contains(_document.LinkId.Value))
            {
                _document.NewLinkId = _document.NewLinkSet.Min();
            }
            else
            {
                _document.NewLinkId = _document.LinkId.Value;
            }
            //CommonDocumentUtilities.SetLastChange(_context, _document);
            _document.Events = CommonDocumentUtilities.GetNewDocumentEvents(_context, (int)EnumEntytiTypes.Document, _document.Id, EnumEventTypes.DeleteLink);
            _operationDb.DeleteDocumentLink(_context, _document);
            return null;
        }

    }
}