using System;
using BL.CrossCutting.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.Database;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.Commands
{
    public class StartWorkDocumentCommand: BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        public StartWorkDocumentCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
        }

        private ChangeWorkStatus Model
        {
            get
            {
                if (!(_param is ChangeWorkStatus))
                {
                    throw new WrongParameterTypeError();
                }
                return (ChangeWorkStatus)_param;
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
            var acc = _operationDb.GetDocumentAccessForUserPosition(_context, Model.DocumentId);
            if (acc == null)
            {
                throw new UserHasNoAccessToDocument();
            }
            acc.IsInWork = true;
            var ea = new EventAccessModel
            {
                DocumentAccess = acc,
                DocumentEvent = new InternalDocumentEvents
                {
                    DocumentId = Model.DocumentId,
                    SourceAgentId = _context.CurrentAgentId,
                    TargetAgentId = _context.CurrentAgentId,
                    SourcePositionId = _context.CurrentPositionId,
                    TargetPositionId = _context.CurrentPositionId,
                    Description = Model.Description,
                    EventType = EnumEventTypes.SetInWork,
                    LastChangeUserId = _context.CurrentAgentId,
                    LastChangeDate = DateTime.Now,
                    Date = DateTime.Now,
                    CreateDate = DateTime.Now,
                }
            };
            _operationDb.SetDocumentInformation(_context, ea);
            return null;
        }

        public override EnumDocumentActions CommandType { get { return EnumDocumentActions.StartWork; } }
    }
}