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
    public class RegisterDocumentCommand: BaseDocumentCommand
    {

        private readonly IDocumentsDbProcess _documentDb;
        private readonly IAdminsDbProcess _adminDb;

        public RegisterDocumentCommand(IDocumentsDbProcess documentDb, IAdminsDbProcess adminDb)
        {
            _documentDb = documentDb;
            _adminDb = adminDb;
        }

        private RegisterDocument Model
        {
            get
            {
                if (!(_param is RegisterDocument))
                {
                    throw new WrongParameterTypeError();
                }
                return (RegisterDocument) _param;
            }
        }

        public override bool CanBeDisplayed()
        {
            try
            {
                _adminDb.VerifyAccess(_context, new VerifyAccess { DocumentActionCode = EnumDocumentActions.RegisterDocument, PositionId = _context.CurrentPositionId });
                if (_document == null || !_document.RegistrationJournalId.HasValue || _document.IsRegistered)
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
            _adminDb.VerifyAccess(_context, new VerifyAccess { DocumentActionCode = EnumDocumentActions.RegisterDocument, PositionId = _context.CurrentPositionId });
            if (_document == null)
            {
                _document = _documentDb.RegisterDocumentPrepare(_context, Model);
            }

            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            if (!_document.RegistrationJournalId.HasValue)
            {
                throw new DictionaryRecordWasNotFound();
            }
            if (_document.IsRegistered)
            {
                throw new DocumentHasAlredyBeenRegistered();
            }
            return true;
        }

        public override object Execute()
        {
            CommonDocumentUtilities.SetLastChangeForDocument(_context, _document);
            _document.IsRegistered = !Model.IsOnlyGetNextNumber;
            _document.RegistrationDate = Model.RegistrationDate;
            bool isNeedGenerateNumber;
            if (Model.RegistrationNumber == null || Model.IsOnlyGetNextNumber)
            {
                _document.RegistrationNumberPrefix = _document.RegistrationJournalPrefixFormula;
                _document.RegistrationNumberSuffix = _document.RegistrationJournalSuffixFormula;
                _document.RegistrationNumber = null;
                isNeedGenerateNumber = true;
            }
            else
            {
                _document.RegistrationNumberPrefix = Model.RegistrationNumberPrefix;
                _document.RegistrationNumberSuffix = Model.RegistrationNumberSuffix;
                _document.RegistrationNumber = Model.RegistrationNumber;
                isNeedGenerateNumber = false;
            }
            var isRepeat = true;
            var isOk = false;

            while (isRepeat)
            {
                if (isNeedGenerateNumber)
                {
                    _documentDb.SetNextDocumentRegistrationNumber(_context, _document);

                }
                _documentDb.UpdateDocument(_context, _document);
                isOk = _documentDb.VerifyDocumentRegistrationNumber(_context, _document);
                isRepeat = isOk ? !isOk : isNeedGenerateNumber;
            }
            if (!isOk)
            {
                _document.IsRegistered = false;
                _document.RegistrationJournalId = null;
                _document.NumerationPrefixFormula = null;
                _document.RegistrationNumber = null;
                _document.RegistrationNumberSuffix = null;
                _document.RegistrationNumberPrefix = null;
                _document.RegistrationDate = null;
                _documentDb.UpdateDocument(_context, _document);
                throw new DocumentCouldNotBeRegistered();
            }
            return Model.DocumentId;
        }

        public override EnumDocumentActions CommandType { get { return EnumDocumentActions.RegisterDocument; } }
    }
}