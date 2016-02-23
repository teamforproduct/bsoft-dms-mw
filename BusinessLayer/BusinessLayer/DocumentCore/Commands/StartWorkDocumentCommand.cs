using System;
using BL.Logic.Common;
using BL.Database.Admins.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Model.AdminCore;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.Commands
{
    public class StartWorkDocumentCommand: BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminsDbProcess _adminDb;

        protected InternalDocumentAccesses DocAccess;

        public StartWorkDocumentCommand(IDocumentOperationsDbProcess operationDb, IAdminsDbProcess adminDb)
        {
            _operationDb = operationDb;
            _adminDb = adminDb;
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
            _adminDb.VerifyAccess(_context, new VerifyAccess { DocumentActionCode = CommandType.ToString() });
            DocAccess = _operationDb.ChangeIsInWorkAccessPrepare(_context, Model.DocumentId);
            if (DocAccess == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            if (DocAccess.IsInWork)
            {
                throw new CouldNotChangeIsInWork();
            }
            return true;
        }

        public override object Execute()
        {
            DocAccess.IsInWork = true;
            DocAccess.LastChangeDate = DateTime.Now;
            DocAccess.LastChangeUserId = _context.CurrentAgentId;
            DocAccess.DocumentEvent = new InternalDocumentEvents
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
                };
            _operationDb.ChangeIsInWorkAccess(_context, DocAccess);
            return null;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.StartWork;
    }
}