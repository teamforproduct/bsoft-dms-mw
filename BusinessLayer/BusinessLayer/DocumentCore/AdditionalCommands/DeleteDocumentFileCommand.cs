using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.Common;
using BL.Logic.FileWorker;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore;

namespace BL.Logic.DocumentCore.AdditionalCommands
{
    public class DeleteDocumentFileCommand: BaseDocumentCommand
    {
        private readonly IAdminService _admin;
        private readonly IDocumentFileDbProcess _operationDb;
        private readonly IFileStore _fStore;

        public DeleteDocumentFileCommand(IAdminService admin, IDocumentFileDbProcess operationDb, IFileStore fStore)
        {
            _admin = admin;
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
            return true;
        }

        public override bool CanExecute()
        {
            if (Model.DocumentId <= 0 || Model.OrderInDocument <= 0)
            {
                throw new WrongParameterValueError();
            }
            _admin.VerifyAccess(_context, CommandType);
            _document = _operationDb.DeleteDocumentFilePrepare(_context, Model);
            if (_document == null)
            {
                throw new UserHasNoAccessToDocument();
            }
            if (_document.DocumentFiles == null || !_document.DocumentFiles.Any())
            {
                throw new UnknownDocumentFile();
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

            _fStore.DeleteFile(_context, docFile);
            _operationDb.DeleteAttachedFile(_context, docFile);
            return null;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.DeleteDocumentFile;
    }
}