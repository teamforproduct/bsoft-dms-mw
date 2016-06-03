﻿using System.Collections.Generic;
using System.Linq;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore.InternalModel;

namespace BL.Logic.DocumentCore.Commands
{
    internal class AddDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentsDbProcess _documentDb;
        private readonly IFileStore _fStore;

        private int? _executorPositionExecutorAgentId;
        private IEnumerable<InternalPropertyLink> _propertyLinksByTemplateDocument;
        private IEnumerable<InternalPropertyLink> _propertyLinksByDocument;

        public AddDocumentCommand(IDocumentsDbProcess documentDb, IFileStore fStore)
        {
            _documentDb = documentDb;
            _fStore = fStore;
        }

        private AddDocumentByTemplateDocument Model
        {
            get
            {
                if (!(_param is AddDocumentByTemplateDocument))
                {
                    throw new WrongParameterTypeError();
                }
                return (AddDocumentByTemplateDocument)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(Context, CommandType);

            _document = _documentDb.AddDocumentPrepare(_context, Model.TemplateDocumentId);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            _executorPositionExecutorAgentId = CommonDocumentUtilities.GetExecutorAgentIdByPositionId(_context, _context.CurrentPositionId);
            if (_executorPositionExecutorAgentId.HasValue)
            {
                _document.ExecutorPositionExecutorAgentId = _executorPositionExecutorAgentId.Value;
            }
            else
            {
                throw new ExecutorAgentForPositionIsNotDefined();
            }

            var filterTemplateByDocument = CommonDocumentUtilities.GetFilterTemplateByDocument(_document).ToArray();

            _propertyLinksByTemplateDocument = CommonSystemUtilities.GetPropertyLinks(_context, EnumObjects.TemplateDocument, filterTemplateByDocument);
            _propertyLinksByDocument = CommonSystemUtilities.GetPropertyLinks(_context, EnumObjects.Documents, filterTemplateByDocument);

            CommonSystemUtilities.VerifyPropertyLinksCompare(_propertyLinksByTemplateDocument, _propertyLinksByDocument);

            return true;
        }

        public override object Execute()
        {
            CommonDocumentUtilities.SetAtrributesForNewDocument(_context, _document);
            CommonDocumentUtilities.SetTaskAtrributesForNewDocument(_context, _document.Tasks, _executorPositionExecutorAgentId.Value);
            CommonDocumentUtilities.SetSendListAtrributesForNewDocument(_context, _document.SendLists, _executorPositionExecutorAgentId.Value, true);
            CommonDocumentUtilities.SetLastChange(_context, _document.RestrictedSendLists);

            Document.Events = CommonDocumentUtilities.GetNewDocumentEvents(_context, null, EnumEventTypes.AddNewDocument, null, "Create");
            Document.Accesses = CommonDocumentUtilities.GetNewDocumentAccesses(_context);

            // prepare file list in Document. It will save it with document in DB
            var toCopy = new Dictionary<InternalDocumentAttachedFile, InternalTemplateAttachedFile>();
            var newOrdNum = 1;
            _document.DocumentFiles.ToList().ForEach(x =>
            {
                x.ExecutorPositionId = _document.ExecutorPositionId;
                x.ExecutorPositionExecutorAgentId = _document.ExecutorPositionExecutorAgentId;

                var fileToCopy = CommonDocumentUtilities.GetNewTemplateAttachedFile(x);

                var newDoc = CommonDocumentUtilities.GetNewDocumentAttachedFile(x, newOrdNum, 1);

                newOrdNum++;
                toCopy.Add(newDoc, fileToCopy);
            });

            // assign new created list of files to document
            _document.DocumentFiles = toCopy.Keys;
            CommonDocumentUtilities.SetLastChange(_context, _document.DocumentFiles);

            //Properties
            _document.Properties = _document.Properties.ToList()
                .Join(_propertyLinksByDocument,
                        pv => new { PropertyId = pv.PropertyLink.PropertyId, pv.PropertyLink.Filers },
                        pl => new { PropertyId = pl.PropertyId, pl.Filers },
                        (pv, pl) => { pv.Id = 0; pv.RecordId = 0; pv.PropertyLinkId = pl.Id; pv.PropertyLink = null; return pv; }).ToList();

            CommonDocumentUtilities.SetLastChange(_context, _document.Properties);

            _documentDb.AddDocument(_context, Document);

            //after saving document in filelist it should be filled DocumentId field. So we can phisical copy files
            foreach (var fl in _document.DocumentFiles)
            {
                var dest = toCopy.Keys.FirstOrDefault(x => x.OrderInDocument == fl.OrderInDocument);
                var src = toCopy[dest];
                _fStore.CopyFile(_context, src, fl);
            }

            return Document.Id;
        }

    }
}