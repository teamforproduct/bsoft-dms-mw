using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.Common;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.TemplateCommands
{
    public class ModifyTemplateFileCommand : BaseDocumentCommand
    {
        private readonly ITemplateDocumentsDbProcess _operationDb;
        private readonly IFileStore _fStore;

        private InternalTemplateAttachedFile _docFile;

        public ModifyTemplateFileCommand(ITemplateDocumentsDbProcess operationDb, IFileStore fStore)
        {
            _operationDb = operationDb;
            _fStore = fStore;
        }

        private ModifyTemplateAttachedFile Model
        {
            get
            {
                if (!(_param is ModifyTemplateAttachedFile))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyTemplateAttachedFile)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType, false);
            _docFile = _operationDb.UpdateFilePrepare(_context, Model.Id);
            if (_docFile == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            return true;
        }

        public override object Execute()
        {
            //            _docFile.OrderInDocument = _operationDb.GetNextFileOrderNumber(_context, Model.DocumentId);
            //fl.FileContent = Convert.FromBase64String(Model.FileData);
            _docFile.Type = Model.Type;
            _docFile.Description = Model.Description;
            //if (Model.PostedFileData != null)
            //{
            //    _docFile.FileSize = Model.FileSize;
            //    _docFile.Extension = Path.GetExtension(Model.FileName ?? "").Replace(".", "");
            //    _docFile.Name = Path.GetFileNameWithoutExtension(Model.FileName);
            //    _docFile.FileType = Model.FileType;
            //    _docFile.PostedFileData = Model.PostedFileData;
            //    _fStore.SaveFile(_context, _docFile);
            //}
            CommonDocumentUtilities.SetLastChange(_context, _docFile);

            _operationDb.UpdateFile(_context, _docFile);

            return new FrontTemplateAttachedFile(_docFile);
        }


    }
}