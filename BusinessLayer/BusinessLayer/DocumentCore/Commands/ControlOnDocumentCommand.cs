using System;
using BL.CrossCutting.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.Commands
{
    public class ControlOnDocumentCommand: BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        public ControlOnDocumentCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
        }

        private ControlOn Model
        {
            get
            {
                if (!(_param is ControlOn))
                {
                    throw new WrongParameterTypeError();
                }
                return (ControlOn) _param;
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
            var docWait = new InternalDocumentWaits
            {
                DocumentId = Model.DocumentId,
                Task = Model.Task,
                DueDate = Model.DueDate,
                AttentionDate = Model.AttentionDate,
                LastChangeUserId = _context.CurrentAgentId,
                LastChangeDate = DateTime.Now,
                OnEvent = new InternalDocumentEvents
                {
                    DocumentId = Model.DocumentId,
                    EventType = EnumEventTypes.ControlOn,
                    Description = Model.Task+" / "+Model.Description,
                    SourcePositionId = _context.CurrentPositionId,
                    SourceAgentId = _context.CurrentAgentId,
                    TargetPositionId = _context.CurrentPositionId,
                    TargetAgentId = _context.CurrentAgentId,
                    LastChangeDate = DateTime.Now,
                    Date = DateTime.Now,
                    CreateDate = DateTime.Now,
                    LastChangeUserId = _context.CurrentAgentId
                }
            };
            _operationDb.AddDocumentWait(_context, docWait);
            return null;
        }

        public override EnumDocumentActions CommandType { get { return EnumDocumentActions.ControlOn; } }
    }
}