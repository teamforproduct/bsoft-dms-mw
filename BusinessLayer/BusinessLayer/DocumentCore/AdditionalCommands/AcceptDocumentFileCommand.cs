using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.AdditionalCommands
{
    public class AcceptDocumentFileCommand : BaseDocumentCommand
    {
        private readonly IDocumentFileDbProcess _operationDb;
        private readonly IFileStore _fStore;

        private InternalDocumentFile _file;

        public AcceptDocumentFileCommand(IDocumentFileDbProcess operationDb, IFileStore fStore)
        {
            _operationDb = operationDb;
            _fStore = fStore;
        }

        private ChangeWorkOutDocumentFile Model
        {
            get
            {
                if (!(_param is ChangeWorkOutDocumentFile))
                {
                    throw new WrongParameterTypeError();
                }
                return (ChangeWorkOutDocumentFile)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            if (CommandType == EnumDocumentActions.AcceptDocumentFile)
            {
                _actionRecords =
                       _document.DocumentFiles.Where(
                           x => !(x.IsWorkedOut ?? true) && !x.IsDeleted && _document.ExecutorPositionId == positionId)
                                                       .Select(x => new InternalActionRecord
                                                       {
                                                           FileId = x.Id,
                                                       });
            }
            else
            {
                _actionRecords =
                       _document.DocumentFiles.Where(x => !x.IsDeleted && !x.IsMainVersion)
                                .Where(x => (x.ExecutorPositionId == positionId && x.Type == EnumFileTypes.Additional)
                                            || (_document.ExecutorPositionId == positionId && x.Type == EnumFileTypes.Main))
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
            //TODO potential two user could add same new version in same time. Probably need to implement CheckOut flag in future
            _document = _operationDb.ModifyDocumentFilePrepare(_context, Model.DocumentId, Model.OrderInDocument, Model.Version);
            if (_document == null)
            {
                throw new UserHasNoAccessToDocument();
            }
            if (_document.DocumentFiles == null || !_document.DocumentFiles.Any())
            {
                throw new UnknownDocumentFile();
            }

            _file = _document.DocumentFiles.First();

            _context.SetCurrentPosition(_document.ExecutorPositionId);

            _adminProc.VerifyAccess(_context, CommandType);

            if (!CanBeDisplayed(_context.CurrentPositionId))
            {
                throw new CouldNotPerformOperation();
            }

            return true;
        }

        public override object Execute()
        {
            _file.IsWorkedOut = true;
            _file.IsMainVersion = true;
            CommonDocumentUtilities.SetLastChange(_context, _file);
            _file.Event = CommonDocumentUtilities.GetNewDocumentEvent(_context, (int)EnumEntytiTypes.Document, _file.DocumentId, EnumEventTypes.AcceptDocumentFile, null, _file.File.FileName, null,null, null, _file.ExecutorPositionId);
            _operationDb.UpdateFileOrVersion(_context, _file);
            return _file.Id;
        }
    }
}