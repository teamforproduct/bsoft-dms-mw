
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
        private readonly ITemplateDbProcess _operationDb;


        public AddTemplateRestrictedSendListCommand(ITemplateDbProcess operationDb)
        {
            _operationDb = operationDb;

        }

        private AddTemplateRestrictedSendList Model
        {
            get
            {
                if (!(_param is AddTemplateRestrictedSendList))
                {
                    throw new WrongParameterTypeError();
                }
                return (AddTemplateRestrictedSendList)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _adminProc.VerifyAccess(_context, CommandType, false);
            if (_operationDb.ExistsTemplateRestrictedSendLists(_context, new FilterTemplateRestrictedSendList {TemplateId = Model.DocumentId,PositionId = Model.PositionId }))
            {
                throw new RecordNotUnique();
            }
            return true;
        }

        public override object Execute()
        {
            var model = new InternalTemplateRestrictedSendList(Model);
            CommonDocumentUtilities.SetLastChange(_context, model);
            return _operationDb.AddOrUpdateTemplateRestrictedSendList(_context, model);

        }


    }
}