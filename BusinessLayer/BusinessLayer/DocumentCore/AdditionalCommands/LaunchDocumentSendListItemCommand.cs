using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Exception;
using System.Linq;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.Common;
using BL.Model.Enums;

namespace BL.Logic.DocumentCore.AdditionalCommands
{
    public class LaunchDocumentSendListItemCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminService _admin;

        protected InternalDocumentSendList DocSendList;

        public LaunchDocumentSendListItemCommand(IDocumentOperationsDbProcess operationDb, IAdminService admin)
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
            _document = _operationDb.LaunchDocumentSendListPrepare(_context, Model);
            _admin.VerifyAccess(_context, CommandType);
            //TODO проверить Source
            DocSendList = _operationDb.DeleteDocumentSendListPrepare(_context, Model);

            _document = _operationDb.ChangeDocumentSendListPrepare(_context, DocSendList.DocumentId);

            _document.SendLists.ToList().Remove(_document.SendLists.FirstOrDefault(x => x.Id == Model));

            CommonDocumentUtilities.VerifySendLists(_document);

            return true;
        }

        public override object Execute()
        {


            return _document.Id;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.LaunchDocumentSendListItem;
    }
}