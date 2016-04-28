using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.Commands
{
    public class RegisterDocumentCommand: BaseDocumentCommand
    {

        private readonly IDocumentsDbProcess _documentDb;

        public RegisterDocumentCommand(IDocumentsDbProcess documentDb)
        {
            _documentDb = documentDb;
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

        public override bool CanBeDisplayed(int positionId)
        {
            if (_document.IsRegistered.HasValue && _document.IsRegistered.Value
                )
            {
                return false;
            }

            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType);
            _document = _documentDb.RegisterDocumentPrepare(_context, Model);

            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            if (!_document.RegistrationJournalId.HasValue)
            {
                throw new DictionaryRecordWasNotFound();
            }
            if (!CanBeDisplayed(_context.CurrentPositionId))
            {
                throw new DocumentHasAlredyBeenRegistered();
            }
            return true;
        }

        public override object Execute()
        {
            CommonDocumentUtilities.SetLastChange(_context, _document);
            _document.IsRegistered = !Model.IsOnlyGetNextNumber;
            _document.RegistrationDate = Model.RegistrationDate;
            bool isNeedGenerateNumber;
            if (Model.RegistrationNumber == null || Model.IsOnlyGetNextNumber)
            {
                var registerModel = _documentDb.RegisterModelDocumentPrepare(_context, Model);
                CommonDocumentUtilities.FormationRegistrationNumberByFormula(_document, registerModel);
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
                    _documentDb.GetNextDocumentRegistrationNumber(_context, _document);

                }
                _documentDb.RegisterDocument(_context, _document);
                isOk = _documentDb.VerifyDocumentRegistrationNumber(_context, _document);
                isRepeat = !isOk && isNeedGenerateNumber;
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
                _documentDb.RegisterDocument(_context, _document);
                throw new DocumentCouldNotBeRegistered();
            }
            return Model.DocumentId;
        }

    }
}