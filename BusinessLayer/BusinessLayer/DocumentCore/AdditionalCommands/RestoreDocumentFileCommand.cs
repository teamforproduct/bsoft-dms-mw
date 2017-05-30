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
    public class RestoreDocumentFileCommand : BaseDocumentCommand
    {
        private readonly IDocumentFileDbProcess _operationDb;
        private readonly IFileStore _fStore;

        private InternalDocumentFile _file;

        public RestoreDocumentFileCommand(IDocumentFileDbProcess operationDb, IFileStore fStore)
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
            
            var qry = _document.DocumentFiles.Where(x => x.IsDeleted && !x.IsContentDeleted && x.ExecutorPositionId == positionId);
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
            _document = _operationDb.RestoreDocumentFilePrepare(_context, Model);
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
            if (_document.IsRegistered.HasValue)
            {
                _file.Event = CommonDocumentUtilities.GetNewDocumentEvent(_context, (int)EnumEntytiTypes.Document, _file.DocumentId,
                    _file.IsMainVersion ? EnumEventTypes.DeleteDocumentFile : EnumEventTypes.DeleteDocumentFileVersion, null, null, _file.File.FileName);
            }
            _operationDb.DeleteDocumentFile(_context, _file);
            return null;
        }
    }
}