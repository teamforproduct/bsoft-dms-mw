using System;
using System.Linq;
using BL.Database.Admins.Interfaces;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.Commands
{
    public class ControlOffDocumentCommand: BaseDocumentCommand
    {
        private readonly IDocumentsDbProcess _documentDb;
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminsDbProcess _adminDb;

        public ControlOffDocumentCommand(IDocumentsDbProcess documentDb, IDocumentOperationsDbProcess operationDb, IAdminsDbProcess adminDb)
        {
            _documentDb = documentDb;
            _operationDb = operationDb;
            _adminDb = adminDb;
        }

        private ControlOff Model
        {
            get
            {
                if (!(_param is ControlOff))
                {
                    throw new WrongParameterTypeError();
                }
                return (ControlOff) _param;
            }
        }

        public override bool CanBeDisplayed()
        {
            return true;
        }

        public override bool CanExecute()
        {
            _document = _operationDb.ControlOffDocumentPrepare(_context, Model.EventId);
            var internalDocumentWait = _document.Waits?.FirstOrDefault();
            if (internalDocumentWait != null && internalDocumentWait.OnEvent?.SourcePositionId == null)
            {
                throw new EventNotFoundOrUserHasNoAccess();
            }
            _context.SetCurrentPosition(_document.Events.First().SourcePositionId);
            _adminDb.VerifyAccess(_context, CommandType);
            return true;
        }

        public override object Execute()
        {
            var docWait = _operationDb.GetDocumentWaitByOnEventId(_context, Model.EventId);

            docWait.ResultTypeId = Model.ResultTypeId;

            docWait.OnEvent = null;
            docWait.OffEvent = new InternalDocumentEvent
            {
//TODO                DocumentId = Model.DocumentId,
                EventType = EnumEventTypes.ControlOff,
                Description = docWait.Task +" / "+Model.Description,
                SourcePositionId = (int)_context.CurrentPositionId,
                SourceAgentId = _context.CurrentAgentId,
                TargetPositionId = _context.CurrentPositionId,
                LastChangeDate = DateTime.Now,
                Date = DateTime.Now,
                CreateDate = DateTime.Now,
            };

            _operationDb.UpdateDocumentWait(_context, docWait);
            return null;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.ControlOff;
    }
}