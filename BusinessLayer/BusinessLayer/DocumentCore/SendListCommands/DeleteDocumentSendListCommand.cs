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
    public class DeleteDocumentSendListCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminService _admin;

        protected InternalDocumentSendList DocSendList;

        public DeleteDocumentSendListCommand(IDocumentOperationsDbProcess operationDb, IAdminService admin)
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
            DocSendList = _operationDb.DeleteDocumentSendListPrepare(_context, Model);

            _context.SetCurrentPosition(DocSendList.SourcePositionId);
            _admin.VerifyAccess(_context, CommandType);

            _document = _operationDb.ChangeDocumentSendListPrepare(_context, DocSendList.DocumentId);

            _document.SendLists.ToList().Remove(_document.SendLists.FirstOrDefault(x => x.Id == Model));

            CommonDocumentUtilities.VerifySendLists(_document);

            return true;
        }

        public override object Execute()
        {
            _operationDb.DeleteDocumentSendList(_context, Model);
            return DocSendList.DocumentId;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.DeleteDocumentSendList;
    }
}