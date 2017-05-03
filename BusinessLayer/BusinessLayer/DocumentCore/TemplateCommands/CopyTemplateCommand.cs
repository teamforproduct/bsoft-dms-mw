
using BL.CrossCutting.Helpers;
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore.InternalModel;
using System.Collections.Generic;
using System.Linq;

namespace BL.Logic.DocumentCore.TemplateCommands
{
    public class CopyTemplateCommand : BaseDocumentCommand
    {
        private readonly ITemplateDocumentsDbProcess _operationDb;
        private InternalTemplateDocument _templateDoc;
        private readonly IFileStore _fStore;

        public CopyTemplateCommand(ITemplateDocumentsDbProcess operationDb, IFileStore fStore)
        {
            _operationDb = operationDb;
            _fStore = fStore;
        }

        private int Model
        {
            get
            {
                if (!(_param is int))
                {
                    throw new WrongParameterTypeError();
                }
                return (int)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType, false);
            _templateDoc = _operationDb.CopyTemplatePrepare(_context, Model);
            if (_templateDoc == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            return true;
        }

        public override object Execute()
        {
            _templateDoc.Name = CommonDocumentUtilities.GetTemplateNameForCopy(_context, _templateDoc.Name);
            CommonDocumentUtilities.SetLastChange(_context, _templateDoc);
            CommonDocumentUtilities.SetLastChange(_context, _templateDoc.Properties);
            CommonDocumentUtilities.SetLastChange(_context, _templateDoc.Tasks);
            CommonDocumentUtilities.SetLastChange(_context, _templateDoc.SendLists);
            CommonDocumentUtilities.SetLastChange(_context, _templateDoc.RestrictedSendLists);
            CommonDocumentUtilities.SetLastChange(_context, _templateDoc.Papers);
            using (var transaction = Transactions.GetTransaction())
            {
                _templateDoc.Id = _operationDb.AddOrUpdateTemplate(_context, _templateDoc);
                _templateDoc.Tasks.ToList().ForEach(x =>
                {
                    x.DocumentId = _templateDoc.Id;
                    x.Id = _operationDb.AddOrUpdateTemplateTask(_context, x);
                });
                _templateDoc.SendLists.ToList().ForEach(x =>
                {
                    x.DocumentId = _templateDoc.Id;
                    if (string.IsNullOrEmpty(x.TaskName))
                        x.TaskId = _templateDoc.Tasks.Where(y => x.TaskName == y.Task).Select(y => (int?)y.Id).FirstOrDefault();
                    x.Id = _operationDb.AddOrUpdateTemplateSendList(_context, x);
                });
                _templateDoc.RestrictedSendLists.ToList().ForEach(x =>
                {
                    x.DocumentId = _templateDoc.Id;
                    x.Id = _operationDb.AddOrUpdateTemplateRestrictedSendList(_context, x);
                });
                _templateDoc.Papers.ToList().ForEach(x =>
                {
                    x.DocumentId = _templateDoc.Id;
                });
                _operationDb.AddTemplateDocumentPapers(_context, _templateDoc.Papers);
                _templateDoc.Files.ToList().ForEach(x =>
                {
                    var newFile = CommonDocumentUtilities.GetNewTemplateDocumentFile(_context,x);
                    newFile.DocumentId = _templateDoc.Id;
                    _fStore.CopyFile(_context, x, newFile);
                    newFile.Id = _operationDb.AddNewFile(_context, newFile);
                });
                transaction.Complete();
            }
            return _templateDoc.Id;

        }


    }
}