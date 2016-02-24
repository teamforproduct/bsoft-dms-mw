using System;
using BL.Logic.Common;
using BL.Database.Admins.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Model.Database;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.Commands
{
    public class FinishWorkDocumentCommand: BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminsDbProcess _adminDb;

        protected InternalDocumentAccesses DocAccess;

        public FinishWorkDocumentCommand(IDocumentOperationsDbProcess operationDb)
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
                return (ChangeWorkStatus) _param;
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
            var acc = _operationDb.ChangeIsInWorkAccessPrepare(_context, Model.DocumentId);
            if (acc == null)
            {
                throw new UserHasNoAccessToDocument();
            }
            acc.IsInWork = false;
            var ea = new EventAccessModel
            {
                DocumentAccess = acc,
                DocumentEvent = new InternalDocumentEvents
                {
                    DocumentId = Model.DocumentId,
                    SourceAgentId = _context.CurrentAgentId,
                    SourcePositionId = _context.CurrentPositionId,
                    TargetPositionId = _context.CurrentPositionId,
                    Description = Model.Description,
                    EventType = EnumEventTypes.SetOutWork,
                    LastChangeUserId = _context.CurrentAgentId,
                    LastChangeDate = DateTime.Now,
                    Date = DateTime.Now,
                    CreateDate = DateTime.Now,
                }
            };
            _operationDb.ChangeIsInWorkAccess(_context, DocAccess);
            return null;
        }

        public override EnumDocumentActions CommandType { get { return EnumDocumentActions.FinishWork; } }
    }
}