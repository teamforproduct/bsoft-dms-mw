using System;
using System.Linq;
using BL.Logic.Common;
using BL.Database.Admins.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Model.AdminCore;
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

        private InternalDocumentAccess _docAccess;

        public FinishWorkDocumentCommand(IDocumentOperationsDbProcess operationDb, IAdminsDbProcess adminDb)
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
                return (ChangeWorkStatus) _param;
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
            _docAccess = _document.Accesses.FirstOrDefault();
            if (!_docAccess.IsInWork)
            {
                throw new CouldNotChangeIsInWork();
            }
            return true;
        }

        public override object Execute()
        {
            _docAccess.IsInWork = false;
            CommonDocumentUtilities.SetLastChange(_context, _docAccess);
            _document.Events = CommonDocumentUtilities.GetNewDocumentEvents(_context, Model.DocumentId, EnumEventTypes.SetOutWork, Model.Description);
             _operationDb.ChangeIsInWorkAccess(_context, _document);
            return null;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.FinishWork;
    }
}