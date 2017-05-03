using System.Collections.Generic;
using System.Linq;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.DictionaryCore.InternalModel;

namespace BL.Logic.DocumentCore.Commands
{
    public class CopyDocumentCommand: BaseDocumentCommand
    {

        private readonly IDocumentsDbProcess _documentDb;
        private readonly IFileStore _fStore;
        InternalDictionaryPositionExecutorForDocument _executorPosition;

        public CopyDocumentCommand(IDocumentsDbProcess documentDb, IFileStore fStore)
        {
            _documentDb = documentDb;
            _fStore = fStore;
        }

        private CopyDocument Model
        {
            get
            {
                if (!(_param is CopyDocument))
                {
                    throw new WrongParameterTypeError();
                }
                return (CopyDocument) _param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType);
            _document = _documentDb.CopyDocumentPrepare(_context, Model.DocumentId);

            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            _executorPosition = CommonDocumentUtilities.GetExecutorAgentIdByPositionId(_context, _context.CurrentPositionId);
            if (_executorPosition?.ExecutorAgentId.HasValue ?? false)
            {
                _document.ExecutorPositionExecutorAgentId = _executorPosition.ExecutorAgentId.Value;
                _document.ExecutorPositionExecutorTypeId = _executorPosition.ExecutorTypeId;
            }
            else
            {
                throw new ExecutorAgentForPositionIsNotDefined();
            }

            return true;
        }

        public override object Execute()
        {
            CommonDocumentUtilities.SetAtrributesForNewDocument(_context, _document);
            CommonDocumentUtilities.SetTaskAtrributesForNewDocument(_context, _document.Tasks, _executorPosition);
            CommonDocumentUtilities.SetSendListAtrributesForNewDocument(_context, _document.SendLists, true);
            CommonDocumentUtilities.SetLastChange(_context, _document.RestrictedSendLists);

            _document.Events = CommonDocumentUtilities.GetNewDocumentEvents(_context, (int)EnumEntytiTypes.Document, null, EnumEventTypes.AddNewDocument);
            _document.Accesses = CommonDocumentUtilities.GetNewDocumentAccesses(_context, (int)EnumEntytiTypes.Document, EnumAccessLevels.PersonallyAndIOAndReferents, Document.Events.First().Accesses);

            // prepare file list in Document. It will save it with document in DB
            var toCopy = new Dictionary<InternalDocumentFile, InternalDocumentFile>();
            _document.DocumentFiles.ToList().ForEach(x =>
            {
                x.ExecutorPositionId = _document.ExecutorPositionId;
                x.ExecutorPositionExecutorAgentId = _document.ExecutorPositionExecutorAgentId;
                x.ExecutorPositionExecutorTypeId = _document.ExecutorPositionExecutorTypeId;
                var newDoc = CommonDocumentUtilities.GetNewDocumentFile(_context,x);
                toCopy.Add(newDoc, x);
            });

            // assign new created list of files to document
            _document.DocumentFiles = toCopy.Keys;
            CommonDocumentUtilities.SetLastChange(_context, _document.DocumentFiles);

            //Properties
            _document.Properties.ToList().ForEach(x => {
                x.Id = 0;
                x.RecordId = 0;
            });

            CommonDocumentUtilities.SetLastChange(_context, _document.Properties);

            _documentDb.AddDocument(_context, _document);

            //after saving document in filelist it should be filled DocumentId field. So we can phisical copy files
            foreach (var fl in _document.DocumentFiles)
            {
                var dest = toCopy.Keys.FirstOrDefault(x => x.OrderInDocument == fl.OrderInDocument);
                var src = toCopy[dest];
                _fStore.CopyFile(_context, src, fl);
            }

            return _document.Id;
        }

    }
}