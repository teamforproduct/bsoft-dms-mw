
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.Common;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.TemplateCommands
{
    public class AddTemplateRestrictedSendListCommand : BaseDocumentCommand
    {
        private readonly ITemplateDocumentsDbProcess _operationDb;


        public AddTemplateRestrictedSendListCommand(ITemplateDocumentsDbProcess operationDb)
        {
            _operationDb = operationDb;

        }

        private AddTemplateDocumentRestrictedSendList Model
        {
            get
            {
                if (!(_param is AddTemplateDocumentRestrictedSendList))
                {
                    throw new WrongParameterTypeError();
                }
                return (AddTemplateDocumentRestrictedSendList)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType, false);
            if (_operationDb.ExistsTemplateDocumentRestrictedSendLists(_context, new FilterTemplateDocumentRestrictedSendList {TemplateId = Model.DocumentId,PositionId = Model.PositionId }))
            {
                throw new RecordNotUnique();
            }
            return true;
        }

        public override object Execute()
        {
            var model = new InternalTemplateDocumentRestrictedSendList(Model);
            CommonDocumentUtilities.SetLastChange(_context, model);
            return _operationDb.AddOrUpdateTemplateRestrictedSendList(_context, model);

        }


    }
}