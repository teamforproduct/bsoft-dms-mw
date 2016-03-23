using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.AdditionalCommands
{
    public class DeleteDocumentTaskCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;        

        public DeleteDocumentTaskCommand(IDocumentOperationsDbProcess operationDb)
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
            _admin.VerifyAccess(_context, CommandType);

            return true;
        }

        public override object Execute()
        {
            _operationDb.DeleteDocumentTask(_context, Model);
            return null;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.DeleteDocumentTask;
    }
}