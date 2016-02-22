using System;
using BL.CrossCutting.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.Commands
{
    public class ControlOffDocumentCommand: BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        public ControlOffDocumentCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
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
            return true;
        }

        public override object Execute()
        {
            var docWait = _operationDb.GetDocumentWaitByOnEventId(_context, Model.EventId);

            docWait.ResultTypeId = Model.ResultTypeId;

            docWait.OnEvent = null;
            docWait.OffEvent = new InternalDocumentEvents
            {
                DocumentId = Model.DocumentId,
                EventType = EnumEventTypes.ControlOff,
                Description = Model.Description,
                SourcePositionId = (int)_context.CurrentPositionId,
                SourceAgentId = _context.CurrentAgentId,
                TargetPositionId = _context.CurrentPositionId,
                TargetAgentId = _context.CurrentAgentId,
                LastChangeDate = DateTime.Now,
                Date = DateTime.Now,
                CreateDate = DateTime.Now,
            };

            _operationDb.UpdateDocumentWait(_context, docWait);
            return null;
        }

        public override EnumDocumentActions CommandType { get { return EnumDocumentActions.ControlOff; } }
    }
}