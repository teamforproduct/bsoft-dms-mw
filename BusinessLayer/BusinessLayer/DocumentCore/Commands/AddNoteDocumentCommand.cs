using System;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.Commands
{
    public class AddNoteDocumentCommand: BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        public AddNoteDocumentCommand(IDocumentOperationsDbProcess operationDb)
        {
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
                return (AddNote) _param;
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
            //TODO переделать под общую схему с оптимизацией
            var evt = new InternalDocumentEvents
            {
                DocumentId = Model.DocumentId,
                Description = Model.Description,
                EventType = EnumEventTypes.AddNote,
                SourceAgentId = _context.CurrentAgentId,
                TargetAgentId = _context.CurrentAgentId,
                SourcePositionId = _context.CurrentPositionId,
                TargetPositionId = _context.CurrentPositionId,
                LastChangeUserId = _context.CurrentAgentId,
                LastChangeDate = DateTime.Now,
                Date = DateTime.Now,
                CreateDate = DateTime.Now
            };

            _operationDb.AddDocumentEvent(_context, evt);
            return null;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.AddNote;
    }
}