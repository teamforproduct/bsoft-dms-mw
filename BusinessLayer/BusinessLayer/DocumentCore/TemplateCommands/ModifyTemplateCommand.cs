
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.TemplateCommands
{
    public class ModifyTemplateCommand: BaseDocumentCommand
    {
        private readonly ITemplateDocumentsDbProcess _operationDb;

        public ModifyTemplateCommand(ITemplateDocumentsDbProcess operationDb)
        {
            _operationDb = operationDb;
           
        }

        private ModifyTemplateDocument Model
        {
            get
            {
                if (!(_param is ModifyTemplateDocument))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyTemplateDocument)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType);

            if (!_operationDb.CanModifyTemplate(_context, Model))
            {
                throw new CouldNotModifyTemplateDocument();
            }
            return true;
        }

        public override object Execute()
        {

            CommonDocumentUtilities.SetLastChange(_context, Model);
            return _operationDb.AddOrUpdateTemplate(_context, Model);
            
        }

       
    }
}