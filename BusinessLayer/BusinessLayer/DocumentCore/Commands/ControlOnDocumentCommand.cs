using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore;

namespace BL.Logic.DocumentCore.Commands
{
    public class ControlOnDocumentCommand: BaseDocumentCommand
    {
        private readonly IDocumentsDbProcess _documentDb;
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminService _admin;

        public ControlOnDocumentCommand(IDocumentsDbProcess documentDb, IDocumentOperationsDbProcess operationDb, IAdminService admin)
        {
            _documentDb = documentDb;
            _operationDb = operationDb;
            _admin = admin;
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

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType);
            _document = _documentDb.GetBlankInternalDocumentById(_context, Model.DocumentId);
            //TODO проверка на контроль с одинаковыми задачами
            return true;
        }

        public override object Execute()
        {
            _document.Waits = CommonDocumentUtilities.GetNewDocumentWaits(_context, Model, EnumEventTypes.ControlOn);
            _operationDb.AddDocumentWaits(_context, _document);
            return null;
        }

    }
}