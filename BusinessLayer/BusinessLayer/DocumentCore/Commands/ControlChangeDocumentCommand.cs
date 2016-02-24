using System;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.Commands
{
    public class ControlChangeDocumentCommand: BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        public ControlChangeDocumentCommand(IDocumentOperationsDbProcess operationDb)
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
                return (ControlChange) _param;
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
            //TODO переделать под общую схему с оптимизацией выборки
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

        public override EnumDocumentActions CommandType { get { return EnumDocumentActions.ControlChange; } }
    }
}