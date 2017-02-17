
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.Common;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.TemplateCommands
{
    public class AddTemplateAccessCommand : BaseDocumentCommand
    {
        private readonly ITemplateDocumentsDbProcess _operationDb;


        public AddTemplateAccessCommand(ITemplateDocumentsDbProcess operationDb)
        {
            _operationDb = operationDb;

        }

        private AddTemplateDocumentAccess Model
        {
            get
            {
                if (!(_param is AddTemplateDocumentAccess))
                {
                    throw new WrongParameterTypeError();
                }
                return (AddTemplateDocumentAccess)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType, false);
            if (_operationDb.ExistsTemplateDocumentAccesses(_context, new FilterTemplateDocumentAccess { TemplateId = Model.DocumentId,PositionId = Model.PositionId }))
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