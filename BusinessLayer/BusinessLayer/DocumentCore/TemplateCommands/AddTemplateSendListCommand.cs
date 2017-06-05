
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
        private readonly ITemplateDbProcess _operationDb;
        private InternalTemplateSendList _sendList;
        public AddTemplateSendListCommand(ITemplateDbProcess operationDb)
        {
            _operationDb = operationDb;

        }

        private AddTemplateSendList Model
        {
            get
            {
                if (!(_param is AddTemplateSendList))
                {
                    throw new WrongParameterTypeError();
                }
                return (AddTemplateSendList)_param;
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
            _sendList = CommonDocumentUtilities.GetNewTemplateSendList(_context, Model);
            return _operationDb.AddTemplateSendList(_context, _sendList);
        }

    }
}