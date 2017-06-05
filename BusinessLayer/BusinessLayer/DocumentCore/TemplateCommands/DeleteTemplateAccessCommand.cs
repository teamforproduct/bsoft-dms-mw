
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.TemplateCommands
{
    public class DeleteTemplateAccessCommand : BaseDocumentCommand
    {
        private readonly ITemplateDbProcess _operationDb;

        public DeleteTemplateAccessCommand(ITemplateDbProcess operationDb)
        {
            _operationDb = operationDb;

        }

        private int Model
        {
            get
            {
                if (!(_param is int))
                {
                    throw new WrongParameterTypeError();
                }
                return (int)_param;
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
            _operationDb.DeleteTemplateAccess(_context, Model);
            return null;
        }

    }
}