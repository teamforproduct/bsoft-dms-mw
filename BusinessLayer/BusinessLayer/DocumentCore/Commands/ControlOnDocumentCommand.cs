using BL.Database.Admins.Interfaces;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.Commands
{
    public class ControlOnDocumentCommand: BaseDocumentCommand
    {
        private readonly IDocumentsDbProcess _documentDb;
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminsDbProcess _adminDb;

        public ControlOnDocumentCommand(IDocumentsDbProcess documentDb, IDocumentOperationsDbProcess operationDb, IAdminsDbProcess adminDb)
        {
            _documentDb = documentDb;
            _operationDb = operationDb;
            _adminDb = adminDb;
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
            _adminDb.VerifyAccess(_context, CommandType);
            _document = _documentDb.GetBlankInternalDocumentById(_context, Model.DocumentId);
            //TODO проверка на контроль с одинаковыми задачами
            return true;
        }

        public override object Execute()
        {
            var docWaits = CommonDocumentUtilities.GetNewDocumentWait(_context, Model, EnumEventTypes.ControlOn);
            _operationDb.AddDocumentWaits(_context, docWaits);
            return null;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.ControlOn;
    }
}