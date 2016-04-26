
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.TemplateCommands
{
    public class AddTemplateCommand: BaseDocumentCommand
    {
        private readonly ITemplateDocumentsDbProcess _operationDb;
   

        public AddTemplateCommand(ITemplateDocumentsDbProcess operationDb)
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

 
            return true;
        }

        public override object Execute()
        {
            CommonDocumentUtilities.SetLastChange(_context, Model);
            return _operationDb.AddOrUpdateTemplate(_context, Model);
         
        }

       
    }
}