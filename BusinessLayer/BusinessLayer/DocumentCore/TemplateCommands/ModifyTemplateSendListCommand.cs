
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.TemplateCommands
{
    public class ModifyTemplateSendListCommand : BaseDocumentCommand
    {
        private readonly ITemplateDocumentsDbProcess _operationDb;
        private InternalTemplateDocumentSendList _sendList;

        public ModifyTemplateSendListCommand(ITemplateDocumentsDbProcess operationDb)
        {
            _operationDb = operationDb;

        }

        private ModifyTemplateDocumentSendList Model
        {
            get
            {
                if (!(_param is ModifyTemplateDocumentSendList))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyTemplateDocumentSendList)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _adminProc.VerifyAccess(_context, CommandType, false);
            return true;
        }

        public override object Execute()
        {
            CommonDocumentUtilities.CorrectModel(_context,Model);
            _sendList = new InternalTemplateDocumentSendList(Model);
            CommonDocumentUtilities.SetLastChange(_context, _sendList);
            return _operationDb.AddOrUpdateTemplateSendList(_context, _sendList);
        }


    }
}