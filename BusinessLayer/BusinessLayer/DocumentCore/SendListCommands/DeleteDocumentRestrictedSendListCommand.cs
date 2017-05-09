using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.SendListCommands
{
    public class DeleteDocumentRestrictedSendListCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        protected InternalDocumentRestrictedSendList DocRestSendList;

        public DeleteDocumentRestrictedSendListCommand(IDocumentOperationsDbProcess operationDb)
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
            DocRestSendList = _operationDb.DeleteDocumentRestrictedSendListPrepare(_context, Model);
            _document = _operationDb.ChangeDocumentSendListPrepare(_context, DocRestSendList.DocumentId);

            _context.SetCurrentPosition(_document.ExecutorPositionId);
            _adminProc.VerifyAccess(_context, CommandType);

            var restrictedSendLists = _document.RestrictedSendLists.ToList();
            restrictedSendLists.Remove(_document.RestrictedSendLists.FirstOrDefault(x => x.Id == Model));
            _document.RestrictedSendLists = restrictedSendLists;

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