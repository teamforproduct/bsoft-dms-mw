using BL.CrossCutting.Common;
using BL.Database.Admins.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.AdminCore;
using BL.Model.DocumentCore.Actions;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.Commands
{
    public class ChangeExecutorDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentsDbProcess _documentDb;
        private readonly IAdminsDbProcess _adminDb;

        public ChangeExecutorDocumentCommand(IDocumentsDbProcess documentDb, IAdminsDbProcess adminDb)
        {
            _documentDb = documentDb;
            _adminDb = adminDb;
        }

        private ChangeExecutor Model
        {
            get
            {
                if (!(_param is ChangeExecutor))
                {
                    throw new WrongParameterTypeError();
                }
                return (ChangeExecutor)_param;
            }
        }

        public override bool CanBeDisplayed()
        {
            //TODO ПЕРЕДЕЛКА
            try
            {
                _document = _documentDb.ChangeExecutorDocumentPrepare(_context, Model);
                _adminDb.VerifyAccess(_context, new VerifyAccess { DocumentActionCode = CommandType.ToString()});
                if (_document == null || _document.ExecutorPositionId != _context.CurrentPositionId)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override bool CanExecute()
        {
            //TODO ОСТАЛЬНЫЕ ПРОВЕРКИ!
            _document = _documentDb.ChangeExecutorDocumentPrepare(_context, Model);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            if (_document.IsRegistered)
            {
                throw new DocumentHasAlredyBeenRegistered();
            }
            _context.SetCurrentPosition(_document.ExecutorPositionId);
            _adminDb.VerifyAccess(_context, new VerifyAccess { DocumentActionCode = CommandType.ToString()});
            return true;
        }

        public override object Execute()
        {
            CommonDocumentUtilities.SetLastChangeForDocument(_context, _document);

            _document.ExecutorPositionId = Model.PositionId;
            _document.AccessLevel = Model.AccessLevel;

            _document.Events = CommonDocumentUtilities.GetEventForChangeExecutorDocument(_context, Model);

            _document.Accesses = CommonDocumentUtilities.GetAccessesForChangeExecutorDocument(_context, Model);

            _documentDb.ChangeExecutorDocument(_context, _document);

            return Model.DocumentId;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.ChangeExecutor;
    }
}