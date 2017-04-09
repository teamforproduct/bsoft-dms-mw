using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.DocumentCore.Actions;
using BL.Model.Enums;
using BL.Model.Exception;
using System.Linq;

namespace BL.Logic.DocumentCore.Commands
{
    public class ChangePositionDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentsDbProcess _documentDb;
        private readonly IDocumentOperationsDbProcess _operationDb;

        public ChangePositionDocumentCommand(IDocumentsDbProcess documentDb, IDocumentOperationsDbProcess operationDb)
        {
            _documentDb = documentDb;
            _operationDb = operationDb;
        }

        private ChangePosition Model
        {
            get
            {
                if (!(_param is ChangePosition))
                {
                    throw new WrongParameterTypeError();
                }
                return (ChangePosition)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            if ((_document.Accesses?.Count() ?? 0) != 0 && !_document.Accesses.Any(x => x.PositionId == positionId && x.IsInWork))
                return false;
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType,false);
            _document = _documentDb.ChangePositionDocumentPrepare(_context, Model);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }

            return true;
        }

        public override object Execute()
        {
            _document.Events = CommonDocumentUtilities.GetNewDocumentEvents(_context, (int)EnumEntytiTypes.Document, Model.DocumentId, EnumEventTypes.ChangePosition, Model.EventDate, Model.Description, null, null, Model.NewPositionId, null, Model.NewPositionId);

            _documentDb.ChangePositionDocument(_context, Model, _document);

            return Model.DocumentId;
        }

    }
}