using System.Linq;
using BL.Database.Admins.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System.Linq;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.Common;
using BL.Model.Enums;

namespace BL.Logic.DocumentCore.SendListCommands
{
    public class DeleteDocumentRestrictedSendListCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminService _admin;

        protected InternalDocumentRestrictedSendList DocRestSendList;

        public DeleteDocumentRestrictedSendListCommand(IDocumentOperationsDbProcess operationDb, IAdminService admin)
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
            _context.SetCurrentPosition(_document.ExecutorPositionId);
            _admin.VerifyAccess(_context, CommandType);

            DocRestSendList = _operationDb.DeleteDocumentRestrictedSendListPrepare(_context, Model);

            _document = _operationDb.ChangeDocumentSendListPrepare(_context, DocRestSendList.DocumentId);

            _document.RestrictedSendLists.ToList().Remove(_document.RestrictedSendLists.FirstOrDefault(x => x.Id == Model));

            CommonDocumentUtilities.VerifySendLists(_document);

            return true;
        }

        public override object Execute()
        {
            _operationDb.DeleteDocumentRestrictedSendList(_context, Model);
            return DocRestSendList.DocumentId;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.DeleteDocumentRestrictedSendList;
    }
}