
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.Common;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Exception;
using System.Collections.Generic;

namespace BL.Logic.DocumentCore.TemplateCommands
{
    public class ModifyTemplateAccessCommand : BaseDocumentCommand
    {
        private readonly ITemplateDocumentsDbProcess _operationDb;

        public ModifyTemplateAccessCommand(ITemplateDocumentsDbProcess operationDb)
        {
            _operationDb = operationDb;

        }

        private ModifyTemplateDocumentAccess Model
        {
            get
            {
                if (!(_param is ModifyTemplateDocumentAccess))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyTemplateDocumentAccess)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType, false);
            if (_operationDb.ExistsTemplateDocumentAccesses(_context, new FilterTemplateDocumentAccess { TemplateId = Model.DocumentId, PositionId = Model.PositionId, NotContainsIDs = new List<int> { Model.Id} }))
            {
                throw new RecordNotUnique();
            }
            return true;
        }

        public override object Execute()
        {
            var model = new InternalTemplateDocumentAccess(Model);
            CommonDocumentUtilities.SetLastChange(_context, model);
            return _operationDb.AddOrUpdateTemplateAccess(_context, model);
        }

    }
}