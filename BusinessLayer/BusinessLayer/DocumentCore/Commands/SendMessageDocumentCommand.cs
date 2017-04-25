﻿using System.Collections.Generic;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.Enums;
using BL.Model.Exception;
using System.Linq;
using BL.Model.DocumentCore.InternalModel;

namespace BL.Logic.DocumentCore.Commands
{
    public class SendMessageDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentsDbProcess _documentDb;
        private readonly IDocumentOperationsDbProcess _operationDb;

        public SendMessageDocumentCommand(IDocumentsDbProcess documentDb, IDocumentOperationsDbProcess operationDb)
        {
            _documentDb = documentDb;
            _operationDb = operationDb;
        }

        private SendMessage Model
        {
            get
            {
                if (!(_param is SendMessage))
                {
                    throw new WrongParameterTypeError();
                }
                return (SendMessage)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            if ((_document.Accesses?.Count() ?? 0) != 0 && !_document.Accesses.Any(x => x.PositionId == positionId && x.IsInWork))
                return false;
            if (_document.AccessesCount > 1)
                return true;
            else
                return false;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType);
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
            var ev = CommonDocumentUtilities.GetNewDocumentEvent(   _context, (int)EnumEntytiTypes.Document, Model.DocumentId, EnumEventTypes.SendMessage, Model.EventDate, Model.Description, null, Model.ParentEventId, taskId, 
                                                                    accessGroups : Model.TargetAccessGroups, isVeryfyDocumentAccess: true);
            if (!ev.Accesses.Any(x=>x.AccessType!=EnumEventAccessTypes.Source))
            {
                throw new NobodyIsChosen();
            }
            _document.Events = new List<InternalDocumentEvent> { ev };
            _operationDb.AddDocumentEvents(_context, _document);
            return null;
        }

    }
}