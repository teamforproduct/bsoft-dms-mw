using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Model.Exception;
using BL.Logic.Common;

namespace BL.Logic.DocumentCore.Commands
{
    internal class DeleteDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentsDbProcess _documentDb;
        private readonly IFileStore _fStore;

        public DeleteDocumentCommand(IDocumentsDbProcess documentDb, IFileStore fStore)
        {
            _documentDb = documentDb;
            _fStore = fStore;
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
            if (_document.Accesses != null && !_document.Accesses.Any(x => x.PositionId == positionId && x.IsInWork))
                return false;
            if (_document.ExecutorPositionId != positionId
                || _document.LinkId != null
                || (_document.IsRegistered.HasValue && _document.IsRegistered.Value)
                || (_document.WaitsCount ?? 0) > 0 || (_document.Waits!=null &&_document.Waits.Any())
                || (_document.SubscriptionsCount ?? 0) > 0 || (_document.Subscriptions!=null &&_document.Subscriptions.Any())
                )
            {
                return false;
            }
            return true;
        }

        public override bool CanExecute()
        {
            _document = _documentDb.DeleteDocumentPrepare(_context, Model);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            _context.SetCurrentPosition(_document.ExecutorPositionId);
            _admin.VerifyAccess(_context, CommandType);

            if (!CanBeDisplayed(_context.CurrentPositionId))
            {
                throw new DocumentCannotBeModifiedOrDeleted();
            }
            return true;
        }

        public override object Execute()
        {
            if (_document.DocumentFiles != null && _document.DocumentFiles.Any())
            {
                _fStore.DeleteAllFileInDocument(_context, _document.Id);
            }

            _documentDb.DeleteDocument(_context, _document.Id);
            return null;
        }

    }
}