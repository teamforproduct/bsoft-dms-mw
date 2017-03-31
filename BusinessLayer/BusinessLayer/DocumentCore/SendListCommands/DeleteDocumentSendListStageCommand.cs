using System.Collections.Generic;
using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.CrossCutting.Helpers;

namespace BL.Logic.DocumentCore.SendListCommands
{
    public class DeleteDocumentSendListStageCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        protected IEnumerable<InternalDocumentSendList> DocSendLists;

        public DeleteDocumentSendListStageCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
        }

        private ModifyDocumentSendListStage Model
        {
            get
            {
                if (!(_param is ModifyDocumentSendListStage))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDocumentSendListStage)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            if (_document.ExecutorPositionId != positionId
                )
            {
                return false;
            }

            return true;
        }

        public override bool CanExecute()
        {
            _document = _operationDb.ChangeDocumentSendListPrepare(_context, Model.DocumentId);

            DocSendLists = _document.SendLists.Where(x => x.Stage == Model.Stage).ToList();

            foreach(var sl in DocSendLists)
            {
                _context.SetCurrentPosition(sl.SourcePositionId);
                _admin.VerifyAccess(_context, CommandType);
                if (!CanBeDisplayed(_context.CurrentPositionId))
                {
                    throw new CouldNotPerformOperation();
                }
                _document.SendLists.ToList().Remove(sl);
            }

            CommonDocumentUtilities.VerifySendLists(_document);

            return true;
        }

        public override object Execute()
        {
            using (var transaction = Transactions.GetTransaction())
            {
                foreach (var sl in DocSendLists)
                {
                    _operationDb.DeleteDocumentSendList(_context, sl);
                }

                var sendLists = _document.SendLists.Where(x => x.Stage > Model.Stage);

                foreach (var sl in sendLists)
                {
                    sl.Stage--;
                    CommonDocumentUtilities.SetLastChange(_context, sl);
                }

                _operationDb.ChangeDocumentSendListStage(_context, sendLists);
                transaction.Complete();
            }
            return null;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.DeleteDocumentSendListStage;
    }
}