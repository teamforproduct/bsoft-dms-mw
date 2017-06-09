using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.Common;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.AdditionalCommands
{
    public class DeleteDocumentFileCommand : BaseDocumentCommand
    {
        private readonly IDocumentFileDbProcess _operationDb;
        private readonly IFileStore _fStore;

        private InternalDocumentFile _file;

        public DeleteDocumentFileCommand(IDocumentFileDbProcess operationDb, IFileStore fStore)
        {
            _operationDb = operationDb;
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
            if ((_document.Accesses?.Count() ?? 0) != 0 && !_document.Accesses.Any(x => x.PositionId == positionId && x.IsInWork))
                return false;
            var qry = _document.DocumentFiles.Where(x => x.ExecutorPositionId == positionId);
            if (CommandType == EnumActions.DeleteDocumentFile)
                qry = qry.Where(x => !x.IsDeleted && x.IsMainVersion);
            else if (CommandType == EnumActions.DeleteDocumentFileVersion)
                qry = qry.Where(x => !x.IsDeleted && !x.IsMainVersion);
            else if (CommandType == EnumActions.DeleteDocumentFileVersionFinal)
                qry = qry.Where(x => x.IsDeleted && !x.IsContentDeleted);
            else return false;
            _actionRecords = qry.Select(x => new InternalActionRecord
            {
                FileId = x.Id,
            });
            if (!_actionRecords.Any())
            {
                return false;
            }
            return true;
        }

        public override bool CanExecute()
        {
            _document = _operationDb.DeleteDocumentFilePrepare(_context, Model);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            if (_document.DocumentFiles == null || !_document.DocumentFiles.Any(x => x.Id == Model))
            {
                throw new UnknownDocumentFile();
            }

            _file = _document.DocumentFiles.Where(x => x.Id == Model).First();

            _context.SetCurrentPosition(_file.ExecutorPositionId);

            _adminProc.VerifyAccess(_context, CommandType);

            if (!CanBeDisplayed(_context.CurrentPositionId))
            {
                throw new CouldNotPerformOperationWithPaper();
            }

            return true;
        }

        public override object Execute()
        {
            CommonDocumentUtilities.SetLastChange(_context, _file);
            if (_file.IsDeleted)
            {
                _operationDb.DeleteDocumentFileFinal(_context, _file);
            }
            else
            {
                if (_document.IsRegistered.HasValue)
                {
                    _file.Event = CommonDocumentUtilities.GetNewDocumentEvent(_context, (int)EnumEntytiTypes.Document, _file.DocumentId,
                        _file.IsMainVersion ? EnumEventTypes.DeleteDocumentFile : EnumEventTypes.DeleteDocumentFileVersion, null, null, _file.File.FileName, _file.EventId);
                }
                _operationDb.DeleteDocumentFile(_context, _file);
            }
            return null;
        }
    }
}