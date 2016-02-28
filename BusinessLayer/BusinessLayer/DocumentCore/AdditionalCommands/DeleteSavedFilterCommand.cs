using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Exception;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.Common;
using BL.Model.Enums;

namespace BL.Logic.DocumentCore.AdditionalCommands
{
    public class DeleteSavedFilterCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminService _admin;

        protected InternalDocumentSendList DocSendList;

        public DeleteSavedFilterCommand(IDocumentOperationsDbProcess operationDb, IAdminService admin)
        {
            _admin = admin;
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

        public override bool CanBeDisplayed()
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
            return DocSendList.DocumentId;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.DeleteSavedFilter;
    }
}