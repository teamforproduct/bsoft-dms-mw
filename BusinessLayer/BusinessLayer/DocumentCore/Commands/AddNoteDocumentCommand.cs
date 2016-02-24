using System;
using BL.Database.Admins.Interfaces;
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
        private readonly IDocumentsDbProcess _documentDb;
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminsDbProcess _adminDb;

        public AddNoteDocumentCommand(IDocumentsDbProcess documentDb, IDocumentOperationsDbProcess operationDb, IAdminsDbProcess adminDb)
        {
            _documentDb = documentDb;
            _operationDb = operationDb;
            _adminDb = adminDb;
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
            _adminDb.VerifyAccess(_context, CommandType);
            _document = _documentDb.GetBlankDocumentId(_context, Model.DocumentId);

            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            return true;
        }
        public override object Execute()
        {
            var events = CommonDocumentUtilities.GetNewDocumentEvent(_context, EnumEventTypes.AddNote, Model.Description, idDocument: Model.DocumentId);
            _operationDb.AddDocumentEvents(_context, events);
            return null;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.AddNote;
    }
}