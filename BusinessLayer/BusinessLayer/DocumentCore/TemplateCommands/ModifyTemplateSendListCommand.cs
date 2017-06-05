
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Exception;
using System.Linq;

namespace BL.Logic.DocumentCore.TemplateCommands
{
    public class ModifyTemplateSendListCommand : BaseDocumentCommand
    {
        private readonly ITemplateDbProcess _operationDb;
        private InternalTemplateSendList _sendList;

        public ModifyTemplateSendListCommand(ITemplateDbProcess operationDb)
        {
            _operationDb = operationDb;

        }

        private ModifyTemplateSendList Model
        {
            get
            {
                if (!(_param is ModifyTemplateSendList))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyTemplateSendList)_param;
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
            return _operationDb.ModifyTemplateSendList(_context, _sendList);
        }


    }
}