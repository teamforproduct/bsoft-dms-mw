using System;
using BL.CrossCutting.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.Commands
{
    public class ControlChangeCommand: BaseCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        public ControlChangeCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
        }

        private ControlChange Model
        {
            get
            {
                if (!(_param is ControlChange))
                {
                    throw new WrongParameterTypeError();
                }
                return _param as ControlChange;
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
            var oldWait = _operationDb.GetDocumentWaitByOnEventId(_context, Model.EventId);

            oldWait.OnEvent = null;
            oldWait.OffEvent = new InternalDocumentEvents
            {
                DocumentId = Model.DocumentId,
                EventType = EnumEventTypes.ControlChange,
                Description = oldWait.Task+" / "+Model.Description,
                SourcePositionId = _context.CurrentPositionId,
                SourceAgentId = _context.CurrentAgentId,
                TargetPositionId = _context.CurrentPositionId,
                TargetAgentId = _context.CurrentAgentId,
                LastChangeDate = DateTime.Now,
                Date = DateTime.Now,
                CreateDate = DateTime.Now,
            };

            _operationDb.UpdateDocumentWait(_context, oldWait);
            // TODO Stas check that oldWait.OffEvent.Id filled during the update operation
            var newWait = new InternalDocumentWaits
            {
                ParentId = oldWait.Id,
                DocumentId = Model.DocumentId,
                Task = Model.Task,
                DueDate = Model.DueDate,
                AttentionDate = Model.AttentionDate,
                OnEventId = oldWait.OffEvent.Id
            };
            _operationDb.AddDocumentWait(_context, newWait);
            return null;
        }
    }
}