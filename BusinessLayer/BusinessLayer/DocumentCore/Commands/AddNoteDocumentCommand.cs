﻿using System.Collections.Generic;
using System.Linq;
using BL.Database.DBModel.Document;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.CrossCutting.Helpers;

namespace BL.Logic.DocumentCore.Commands
{
    public class AddNoteDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentsDbProcess _documentDb;
        private readonly IDocumentOperationsDbProcess _operationDb;

        public AddNoteDocumentCommand(IDocumentsDbProcess documentDb, IDocumentOperationsDbProcess operationDb)
        {
            _documentDb = documentDb;
            _operationDb = operationDb;
        }

        private AddNote Model
        {
            get
            {
                if (!(_param is AddNote))
                {
                    throw new WrongParameterTypeError();
                }
                return (AddNote)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            if ((_document.Accesses?.Count() ?? 0) != 0 && !_document.Accesses.Any(x => x.PositionId == positionId && x.IsInWork))
                return false;
            return true;
        }

        public override bool CanExecute()
        {
            _adminProc.VerifyAccess(_context, CommandType);
            _document = _operationDb.AddNoteDocumentPrepare(_context, Model);

            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            return true;
        }
        public override object Execute()
        {
            var taskId = CommonDocumentUtilities.GetDocumentTaskOrCreateNew(_context, _document, Model.Task);
            _document.Events = CommonDocumentUtilities.GetNewDocumentEvents(_context, (int)EnumEntytiTypes.Document, Model.DocumentId, EnumEventTypes.AddNote, Model.EventDate, Model.Description, null, Model.ParentEventId, taskId,
                accessGroups: Model.TargetCopyAccessGroups);
            using (var transaction = Transactions.GetTransaction())
            {
                _operationDb.AddDocumentEvents(_context, _document);
                Model.AddDocumentFiles.ForEach(x => { x.DocumentId = _document.Id; x.EventId = _document.Events.Select(y=>y.Id).First(); });
                _documentProc.ExecuteAction(EnumDocumentActions.AddDocumentFile, _context, Model.AddDocumentFiles);
                transaction.Complete();
            }
            return null;
        }

    }
}