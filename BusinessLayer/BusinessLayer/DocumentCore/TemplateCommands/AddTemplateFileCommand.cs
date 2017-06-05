using System.IO;
using BL.CrossCutting.Context;
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.Common;
using BL.Logic.SystemServices.QueueWorker;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DocumentCore.Filters;
using BL.Model.Enums;
using BL.Logic.SystemServices.TempStorage;
using BL.CrossCutting.DependencyInjection;
using BL.Model.Common;

namespace BL.Logic.DocumentCore.TemplateCommands
{
    public class AddTemplateFileCommand : BaseDocumentCommand
    {
        private readonly ITemplateDbProcess _operationDb;
        private readonly IFileStore _fStore;
        private readonly IQueueWorkerService _queueWorkerService;
        private BaseFile _file;
        

        public AddTemplateFileCommand(ITemplateDbProcess operationDb, IFileStore fStore, IQueueWorkerService queueWorkerService)
        {
            _operationDb = operationDb;
            _fStore = fStore;
            _queueWorkerService = queueWorkerService;
        }

        private AddTemplateFile Model
        {
            get
            {
                if (!(_param is AddTemplateFile))
                {
                    throw new WrongParameterTypeError();
                }
                return (AddTemplateFile)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _file = DmsResolver.Current.Get<ITempStorageService>().GetStoreObject(Model.TmpFileId) as BaseFile;
            if (_file == null)
                throw new CannotAccessToFile();
            _adminProc.VerifyAccess(_context, CommandType, false);

            if (!_operationDb.CanAddTemplateFile(_context, Model, _file))
            {
                throw new CouldNotAddTemplateFile();
            }
            if (_operationDb.ExistsTemplateFiles(_context, new FilterTemplateFile
                { TemplateId = Model.DocumentId, NameExactly = _file.Name, ExtentionExactly = _file.Extension }))
            {
                throw new RecordNotUnique();
            }
            return true;

           
        }

        public override object Execute()
        {
            var att = CommonDocumentUtilities.GetNewTemplateFile(_context, Model, _file);
            att.OrderInDocument = _operationDb.GetNextFileOrderNumber(_context, Model.DocumentId);
            _fStore.SaveFile(_context, att);
            _operationDb.AddTemplateFile(_context, att);
            var admContext = new AdminContext(_context);
            _queueWorkerService.AddNewTask(admContext, () =>
            {
                if (_fStore.CreatePdfFile(admContext, att))
                {
                    _operationDb.ModifyTemplateFilePdfView(admContext, att);
                }
            });

            return att.Id;
        }


    }
}