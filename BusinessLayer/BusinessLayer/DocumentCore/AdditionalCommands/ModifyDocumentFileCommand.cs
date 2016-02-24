using System;
using System.IO;
using System.Linq;
using BL.Database.Admins.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Logic.SystemLogic;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.AdditionalCommands
{
    public class ModifyDocumentFileCommand: BaseDocumentAdditionCommand
    {
        private readonly IAdminsDbProcess _adminDb;
        private readonly IDocumentFileDbProcess _operationDb;
        private readonly IFileStore _fStore;

        private InternalDocumentAttachedFile fl;

        public ModifyDocumentFileCommand(IAdminsDbProcess adminDb, IDocumentFileDbProcess operationDb, IFileStore fStore)
        {
            _adminDb = adminDb;
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

        public override bool CanBeDisplayed()
        {
            return true;
        }

        public override bool CanExecute()
        {
            _adminDb.VerifyAccess(_context, CommandType);

            //TODO potential two user could add same new version in same time. Probably need to implement CheckOut flag in future
            _document = _operationDb.ModifyDocumentFilePrepare(_context, Model.DocumentId, Model.OrderInDocument);
            if (_document == null)
            {
                throw new UserHasNoAccessToDocument();
            }
            if (_document.DocumentFiles == null || !_document.DocumentFiles.Any())
            {
                throw new UnknownDocumentFile();
            }
            fl = _document.DocumentFiles.First();
            return true;
        }

        public override object Execute()
        {
            fl.FileContent = Convert.FromBase64String(Model.FileData);
            fl.FileType = Model.FileType;
            fl.Extension = Path.GetExtension(Model.FileName).Replace(".", "");
            fl.Name = Path.GetFileNameWithoutExtension(Model.FileName);
            fl.IsAdditional = fl.IsAdditional;
            //fl.Date = DateTime.Now;
            _fStore.SaveFile(_context, fl);
            CommonDocumentUtilities.SetLastChange(_context, fl);

            _operationDb.UpdateFileOrVersion(_context, fl);
            // имя модифировавшего пользователя в данном случае не заполнено!
            return new FrontDocumentAttachedFile(fl);
        }

        public override EnumDocumentAdditionActions CommandType => EnumDocumentAdditionActions.ModifyDocumentFile;
    }
}