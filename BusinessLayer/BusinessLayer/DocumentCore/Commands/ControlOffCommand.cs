using System;
using BL.CrossCutting.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.Commands
{
    public class ControlOffCommand: BaseCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        public ControlOffCommand(IDocumentOperationsDbProcess operationDb)
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
                return _param as ControlOff;
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
    }
}