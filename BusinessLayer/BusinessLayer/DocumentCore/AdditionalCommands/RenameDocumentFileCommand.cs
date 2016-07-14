using System;
using System.IO;
using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System.Collections.Generic;

namespace BL.Logic.DocumentCore.AdditionalCommands
{
    public class RenameDocumentFileCommand : BaseDocumentCommand
    {
        private readonly IDocumentFileDbProcess _operationDb;
        private readonly IFileStore _fStore;

        public RenameDocumentFileCommand(IDocumentFileDbProcess operationDb, IFileStore fStore)
        {
            _operationDb = operationDb;
            _fStore = fStore;
        }

        private ModifyDocumentFile Model
        {
            get
            {
                if (!(_param is ModifyDocumentFile))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDocumentFile)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            _actionRecords =
                   _document.DocumentFiles.Where(x => x.IsMainVersion && !x.IsDeleted).Where(
                       x =>
                           (x.ExecutorPositionId == positionId && x.IsAdditional)
                           || (_document.ExecutorPositionId == positionId && !x.IsAdditional))
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
            _admin.VerifyAccess(_context, CommandType);

            //TODO potential two user could add same new version in same time. Probably need to implement CheckOut flag in future
            _document = _operationDb.RenameDocumentFilePrepare(_context, Model.DocumentId, Model.OrderInDocument);
            if (_document == null)
            {
                throw new UserHasNoAccessToDocument();
            }
            if (_document.DocumentFiles == null || !_document.DocumentFiles.Any())
            {
                throw new UnknownDocumentFile();
            }

            if (!CanBeDisplayed(_context.CurrentPositionId))
            {
                throw new CouldNotPerformOperation();
            }

            return true;
        }

        public override object Execute()
        {
            var oldName = _document.DocumentFiles.First().Name;
            var extension = _document.DocumentFiles.First().Extension;

            Model.FileName = Path.GetFileNameWithoutExtension(Model.FileName);

            foreach (var file in _document.DocumentFiles)
            {
                _fStore.RenameFile(_context, file, Model.FileName);

                file.Name = Model.FileName;
                CommonDocumentUtilities.SetLastChange(_context, file);
            }

            var events = CommonDocumentUtilities.GetNewDocumentEvents(_context, Model.DocumentId, EnumEventTypes.RanameDocumentFile, null, oldName + "." + extension, Model.FileName + "." + extension);

            _operationDb.RenameFile(_context, _document.DocumentFiles, events);

            return null;
        }
    }
}