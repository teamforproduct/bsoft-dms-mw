using System;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Database.Admins.Interfaces;
using BL.Model.AdminCore;
using BL.Model.DocumentCore.IncomingModel;
using System.Linq;
using BL.Logic.Common;
using System.Collections.Generic;

namespace BL.Logic.DocumentCore.AdditionalCommands
{
    public class DeleteDocumentSendListStageCommand : BaseDocumentAdditionCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminsDbProcess _adminDb;

        protected IEnumerable<InternalDocumentSendLists> DocSendLists;

        public DeleteDocumentSendListStageCommand(IDocumentOperationsDbProcess operationDb, IAdminsDbProcess adminDb)
        {
            _adminDb = adminDb;
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

        public override bool CanBeDisplayed()
        {
            return true;
        }

        public override bool CanExecute()
        {
            _context.SetCurrentPosition(_document.ExecutorPositionId);
            _adminDb.VerifyAccess(_context, new VerifyAccess { DocumentActionCode = CommandType.ToString() });

            _document = _operationDb.ChangeDocumentSendListPrepare(_context, Model.DocumentId);

            DocSendLists = _document.SendLists.Where(x => x.Stage == Model.Stage).ToList();

            foreach(var sl in DocSendLists)
            {
                _document.SendLists.ToList().Remove(sl);
            }

            CommonDocumentUtilities.VerifySendLists(_document);

            return true;
        }

        public override object Execute()
        {

            foreach (var sl in DocSendLists)
            {
                _operationDb.DeleteDocumentSendList(_context, sl.Id);
            }

            var sendLists = _document.SendLists.Where(x => x.Stage > Model.Stage);

            foreach (var sl in sendLists)
            {
                sl.Stage--;
                CommonDocumentUtilities.SetLastChange(_context, sl);
            }

            _operationDb.ChangeDocumentSendListStage(_context, sendLists);

            return null;
        }

        public override EnumDocumentAdditionActions CommandType => EnumDocumentAdditionActions.DeleteDocumentSendListStage;
    }
}