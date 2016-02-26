using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Exception;
using BL.Database.Admins.Interfaces;
using BL.Model.AdminCore;
using System.Linq;
using BL.Logic.Common;
using BL.Model.Enums;

namespace BL.Logic.DocumentCore.AdditionalCommands
{
    public class DeleteSavedFilterCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminsDbProcess _adminDb;

        protected InternalDocumentSendList DocSendList;

        public DeleteSavedFilterCommand(IDocumentOperationsDbProcess operationDb, IAdminsDbProcess adminDb)
        {
            _adminDb = adminDb;
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
            //_adminDb.VerifyAccess(_context, CommandType);

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