using System;
using System.Linq;
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
            _adminDb.VerifyAccess(_context, CommandType);
            _document = _operationDb.ChangeIsInWorkAccessPrepare(_context, Model.DocumentId);
            if (_document?.Accesses == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            DocAccess = _document.Accesses.FirstOrDefault();
            if (DocAccess.IsInWork)
            {
                throw new CouldNotChangeIsInWork();
            }
            DocAccess = _document.Accesses.FirstOrDefault();
            return true;
        }

        public override object Execute()
        {
            DocAccess.IsInWork = true;
            CommonDocumentUtilities.SetLastChange(_context, DocAccess);
            _document.Events = CommonDocumentUtilities.GetNewDocumentEvent(_context, EnumEventTypes.SetInWork, Model.Description, idDocument: Model.DocumentId);
            _operationDb.ChangeIsInWorkAccess(_context, _document);
            return null;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.StartWork;
    }
}