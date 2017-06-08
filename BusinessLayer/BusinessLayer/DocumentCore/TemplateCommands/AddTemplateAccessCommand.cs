
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
        private readonly ITemplateDbProcess _operationDb;


        public AddTemplateAccessCommand(ITemplateDbProcess operationDb)
        {
            _operationDb = operationDb;

        }

        private AddTemplateAccess Model
        {
            get
            {
                if (!(_param is AddTemplateAccess))
                {
                    throw new WrongParameterTypeError();
                }
                return (AddTemplateAccess)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _adminProc.VerifyAccess(_context, CommandType, false);
            if (_operationDb.ExistsTemplateAccesses(_context, new FilterTemplateAccess { TemplateId = Model.DocumentId,PositionId = Model.PositionId }))
            {
                throw new RecordNotUnique();
            }
            return true;
        }

        public override object Execute()
        {
            var model = new InternalTemplateAccess(Model);
            CommonDocumentUtilities.SetLastChange(_context, model);
            return _operationDb.AddOrUpdateTemplateAccess(_context, model);

        }


    }
}