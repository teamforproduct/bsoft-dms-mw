using BL.Database.Documents.Interfaces;
using BL.Model.Exception;
using BL.Logic.Common;
using BL.Model.Enums;

namespace BL.Logic.DocumentCore.AdditionalCommands
{
    public class DeleteSavedFilterCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        public DeleteSavedFilterCommand(IDocumentOperationsDbProcess operationDb)
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
            //TODO Добавить проверки
            //_context.SetCurrentPosition(_document.ExecutorPositionId);
            //_admin.VerifyAccess(_context, CommandType);

            return true;
        }

        public override object Execute()
        {
            _operationDb.DeleteSavedFilter(_context, Model);
            return null;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.DeleteSavedFilter;
    }
}