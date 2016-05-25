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

        private InternalDocumentAttachedFile _file;

        public DeleteDocumentFileCommand(IDocumentFileDbProcess operationDb, IFileStore fStore)
        {
            _operationDb = operationDb;
            _fStore = fStore;
        }

        private FilterDocumentFileIdentity Model
        {
            get
            {
                if (!(_param is FilterDocumentFileIdentity))
                {
                    throw new WrongParameterTypeError();
                }
                return (FilterDocumentFileIdentity)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            _actionRecords =
                          _document.DocumentFiles.Where(
                              x =>
                                  x.ExecutorPositionId == positionId)
                                                          .Select(x => new InternalActionRecord
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
            if (Model.DocumentId <= 0 || Model.OrderInDocument <= 0)
            {
                throw new WrongParameterValueError();
            }

            _document = _operationDb.DeleteDocumentFilePrepare(_context, Model);
            if (_document == null)
            {
                throw new UserHasNoAccessToDocument();
            }
            if (_document.DocumentFiles == null || !_document.DocumentFiles.Any())
            {
                throw new UnknownDocumentFile();
            }

            _file = _document.DocumentFiles.First();

            _context.SetCurrentPosition(_file.ExecutorPositionId);
            _admin.VerifyAccess(_context, CommandType);

            if (!CanBeDisplayed(_context.CurrentPositionId))
            {
                throw new CouldNotPerformOperationWithPaper();
            }

            return true;
        }

        public override object Execute()
        {
            var docFile = new InternalDocumentAttachedFile
            {
                DocumentId = Model.DocumentId,
                OrderInDocument = Model.OrderInDocument
            };

            try
            {
                _fStore.DeleteFile(_context, docFile);
            }
            catch (CannotAccessToFile ex)
            {

            }
            docFile.Events = CommonDocumentUtilities.GetNewDocumentEvents(_context, docFile.DocumentId, EnumEventTypes.DeleteDocumentFile, null, _file.Name + "." + _file.Extension);
            _operationDb.DeleteAttachedFile(_context, docFile);
            return null;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.DeleteDocumentFile;
    }
}