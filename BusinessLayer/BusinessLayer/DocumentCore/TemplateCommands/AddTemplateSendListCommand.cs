
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.TemplateCommands
{
    public class AddTemplateSendListCommand : BaseDocumentCommand
    {
        private readonly ITemplateDocumentsDbProcess _operationDb;

        public AddTemplateSendListCommand(ITemplateDocumentsDbProcess operationDb)
        {
            _operationDb = operationDb;

        }

        private AddTemplateDocumentSendList Model
        {
            get
            {
                if (!(_param is AddTemplateDocumentSendList))
                {
                    throw new WrongParameterTypeError();
                }
                return (AddTemplateDocumentSendList)_param;
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
            CommonDocumentUtilities.CorrectModel(_context, Model);
            var _sendList = CommonDocumentUtilities.GetNewTemplateDocumentSendList(_context, Model);
            CommonDocumentUtilities.SetLastChange(_context, _sendList);
            return _operationDb.AddOrUpdateTemplateSendList(_context, _sendList);
        }

    }
}