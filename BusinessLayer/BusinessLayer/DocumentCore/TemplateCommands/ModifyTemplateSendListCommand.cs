
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.TemplateCommands
{
    public class ModifyTemplateSendListCommand: BaseDocumentCommand
    {
        private readonly ITemplateDocumentsDbProcess _operationDb;

        public ModifyTemplateSendListCommand(ITemplateDocumentsDbProcess operationDb)
        {
            _operationDb = operationDb;
           
        }

        private ModifyTemplateDocumentSendLists Model
        {
            get
            {
                if (!(_param is ModifyTemplateDocumentSendLists))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyTemplateDocumentSendLists)_param;
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
            return _operationDb.AddOrUpdateTemplateSendList(_context, Model);
            
        }

       
    }
}