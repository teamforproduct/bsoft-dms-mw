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
    public class DeleteDocumentFileVersionCommand : BaseDocumentCommand
    {
        private readonly IDocumentFileDbProcess _operationDb;
        private readonly IFileStore _fStore;

        private InternalDocumentFile _file;

        public DeleteDocumentFileVersionCommand(IDocumentFileDbProcess operationDb, IFileStore fStore)
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
            //if (CommandType == EnumDocumentActions.DeleteDocumentFileVersionRecord)
            //{
            //    _actionRecords =
            //              _document.DocumentFiles.Where(
            //                  x => x.IsDeleted &&
            //                      x.ExecutorPositionId == positionId)
            //                                              .Select(x => new InternalActionRecord
            //                                              {
            //                                                  FileId = x.Id,
            //                                              });
            //}
            //else
            {
                _actionRecords =
                              _document.DocumentFiles.Where(
                                  x => (!x.IsMainVersion && !x.IsDeleted) &&
                                      x.ExecutorPositionId == positionId)
                                                              .Select(x => new InternalActionRecord
                                                              {
                                                                  FileId = x.Id,
                                                              });
            }
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
            if (_document.DocumentFiles == null || !_document.DocumentFiles.Any(x=>x.Id == Model))
            {
                throw new UnknownDocumentFile();
            }

            _file = _document.DocumentFiles.First(x => x.Id == Model);

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
            var docFile = new InternalDocumentFile
            {
                ClientId = _document.ClientId,
                EntityTypeId = _document.EntityTypeId,
                DocumentId = _file.DocumentId,
                OrderInDocument = _file.OrderInDocument,
                Version = _file.Version,
                IsDeleted = _file.IsDeleted
            };

            try
            {
                //_fStore.DeleteFile(_context, docFile);
            }
            catch (CannotAccessToFile ex)
            {

            }
            //if (_document.IsRegistered.HasValue)
            //{
            //    docFile.Events = CommonDocumentUtilities.GetNewDocumentEvents(_context, docFile.DocumentId, EnumEventTypes.DeleteDocumentFileVersion, null, null, _file.Name + "." + _file.Extension);
            //}
            _operationDb.DeleteAttachedFile(_context, docFile);
            return null;
        }
    }
}