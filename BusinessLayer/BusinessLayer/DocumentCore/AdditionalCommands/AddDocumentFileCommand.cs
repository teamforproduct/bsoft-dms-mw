using System;
using System.Collections.Generic;
using System.IO;
using BL.Database.Documents.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.Common;
using BL.Logic.FileWorker;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore;

namespace BL.Logic.DocumentCore.AdditionalCommands
{
    public class AddDocumentFileCommand: BaseDocumentCommand
    {
        private readonly IAdminService _admin;
        private readonly IDocumentFileDbProcess _operationDb;
        private readonly IFileStore _fStore;

        public AddDocumentFileCommand(IAdminService admin, IDocumentFileDbProcess operationDb, IFileStore fStore)
        {
            _admin = admin;
            _operationDb = operationDb;
            _fStore = fStore;
        }

        private ModifyDocumentFiles Model
        {
            get
            {
                if (!(_param is ModifyDocumentFiles))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDocumentFiles)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType);
            _document = _operationDb.AddDocumentFilePrepare(_context, Model.DocumentId);
            if (_document == null)
            {
                throw new UserHasNoAccessToDocument();
            }
            return true;
        }

        public override object Execute()
        {
            var res = new List<FrontDocumentAttachedFile>();
            foreach (var file in Model.Files)
            {
                var att = new InternalDocumentAttachedFile
                {
                    DocumentId = Model.DocumentId,
                    Date = DateTime.Now,
                    FileContent = Convert.FromBase64String(file.FileData),
                    IsAdditional = file.IsAdditional,
                    Version = 1,
                    FileType = file.FileType,
                    FileSize = file.FileSize,
                    OrderInDocument = _operationDb.GetNextFileOrderNumber(_context, Model.DocumentId),
                    Name = Path.GetFileNameWithoutExtension(file.FileName),
                    Extension = Path.GetExtension(file.FileName).Replace(".", ""),
                    WasChangedExternal = false
                };
                _fStore.SaveFile(_context, att);
                CommonDocumentUtilities.SetLastChange(_context, att);
                _operationDb.AddNewFileOrVersion(_context, att);
                // Модель фронта содержит дополнительно только одно поле - пользователя, который последний модифицировал файл. 
                // это поле не заполняется, иначе придется после каждого добавления файла делать запрос на выборку этого файла из таблицы
                // как вариант можно потому будет добавить получение имени текущего пользователя вначале и дописывать его к модели
                res.Add(new FrontDocumentAttachedFile(att));
            }

            return res;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.AddDocumentFile;
    }
}