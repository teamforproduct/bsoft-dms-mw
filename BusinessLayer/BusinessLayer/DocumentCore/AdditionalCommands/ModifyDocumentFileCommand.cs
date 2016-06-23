using System;
using System.IO;
using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.Common;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.AdditionalCommands
{
    public class ModifyDocumentFileCommand : BaseDocumentCommand
    {
        private readonly IDocumentFileDbProcess _operationDb;
        private readonly IFileStore _fStore;

        private InternalDocumentAttachedFile _file;

        public ModifyDocumentFileCommand(IDocumentFileDbProcess operationDb, IFileStore fStore)
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

            _admin.VerifyAccess(_context, CommandType);

            if (!CanBeDisplayed(_context.CurrentPositionId))
            {
                throw new CouldNotPerformOperation();
            }

            return true;
        }

        public override object Execute()
        {
            _file.Description = Model.Description;

            CommonDocumentUtilities.SetLastChange(_context, _file);
            _file.Events = CommonDocumentUtilities.GetNewDocumentEvents(_context, _file.DocumentId, EnumEventTypes.ModifyDocumentFile, null, _file.Name + "." + _file.Extension);
            _operationDb.UpdateFileOrVersion(_context, _file);
            return _file.Id;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.ModifyDocumentFile;
    }
}