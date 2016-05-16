using System;
using System.Collections.Generic;
using System.IO;
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.Common;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System.Linq;

namespace BL.Logic.DocumentCore.AdditionalCommands
{
    public class AddDocumentFileCommand: BaseDocumentCommand
    {
        private readonly IDocumentFileDbProcess _operationDb;
        private readonly IFileStore _fStore;

        public AddDocumentFileCommand(IDocumentFileDbProcess operationDb, IFileStore fStore)
        {
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

            if (Model.Files.Any(x=>!x.IsAdditional) && _document.ExecutorPositionId != _context.CurrentPositionId)
            {
                throw new CouldNotPerformOperation();
            }

            return true;
        }

        public override object Execute()
        {
            var executorPositionExecutorAgentId = CommonDocumentUtilities.GetExecutorAgentIdByPositionId(_context, _context.CurrentPositionId);
            if (!executorPositionExecutorAgentId.HasValue)
            {
                throw new ExecutorAgentForPositionIsNotDefined();
            }
            var res = new List<int>();
            foreach (var file in Model.Files)
            {
                var att = new InternalDocumentAttachedFile
                {
                    DocumentId = Model.DocumentId,
                    Date = DateTime.Now,
                    //FileContent = Convert.FromBase64String(file.FileData),
                    PostedFileData = file.PostedFileData,
                    IsAdditional = file.IsAdditional,
                    FileType = file.FileType,
                    FileSize = file.FileSize,
                    Name = Path.GetFileNameWithoutExtension(file.FileName),
                    Extension = Path.GetExtension(file.FileName).Replace(".", ""),
                    WasChangedExternal = false,
                    ExecutorPositionId = _context.CurrentPositionId,
                    ExecutorPositionExecutorAgentId = executorPositionExecutorAgentId.Value
                };
                var ordInDoc = _operationDb.CheckFileForDocument(_context, Model.DocumentId, att.Name, att.Extension);
                if (ordInDoc == -1)
                {
                    att.Version = 1;
                    att.OrderInDocument = _operationDb.GetNextFileOrderNumber(_context, Model.DocumentId);
                }
                else
                {
                    att.Version = _operationDb.GetFileNextVersion(_context, att.DocumentId, ordInDoc);
                    att.OrderInDocument = ordInDoc;
                }


                _fStore.SaveFile(_context, att);
                CommonDocumentUtilities.SetLastChange(_context, att);
                att.Events = CommonDocumentUtilities.GetNewDocumentEvents(_context, att.DocumentId, EnumEventTypes.AddDocumentFile,null, att.Name + "." + att.Extension);
                res.Add(_operationDb.AddNewFileOrVersion(_context, att));
                // Модель фронта содержит дополнительно только одно поле - пользователя, который последний модифицировал файл. 
                // это поле не заполняется, иначе придется после каждого добавления файла делать запрос на выборку этого файла из таблицы
                // как вариант можно потому будет добавить получение имени текущего пользователя вначале и дописывать его к модели
                //res.Add(new FrontDocumentAttachedFile(att));
            }


            return res;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.AddDocumentFile;
    }
}