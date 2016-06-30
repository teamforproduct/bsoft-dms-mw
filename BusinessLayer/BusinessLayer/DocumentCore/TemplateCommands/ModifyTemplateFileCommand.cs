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

namespace BL.Logic.DocumentCore.TemplateCommands
{
    public class ModifyTemplateFileCommand : BaseDocumentCommand
    {
        private readonly ITemplateDocumentsDbProcess _operationDb;
        private readonly IFileStore _fStore;

        private InternalTemplateAttachedFile fl;

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

            return true;
        }

        public override object Execute()
        {

            fl.DocumentId = Model.DocumentId;
            fl.OrderInDocument = _operationDb.GetNextFileOrderNumber(_context, Model.DocumentId);
            //fl.FileContent = Convert.FromBase64String(Model.FileData);
            fl.FileType = Model.FileType;
            fl.FileSize = Model.FileSize;
            fl.Extension = Path.GetExtension(Model.FileName ?? "").Replace(".", "");
            fl.Name = Path.GetFileNameWithoutExtension(Model.FileName);
            fl.IsAdditional = fl.IsAdditional;
            fl.PostedFileData = Model.PostedFileData;
            fl.Description = Model.Description;
            _fStore.SaveFile(_context, fl);
            CommonDocumentUtilities.SetLastChange(_context, fl);
            _operationDb.UpdateFile(_context, fl);

            return new FrontTemplateAttachedFile(fl);
        }


    }
}