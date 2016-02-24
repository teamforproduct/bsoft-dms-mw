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
    public class DeleteDocumentSendListCommand : BaseDocumentAdditionCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminsDbProcess _adminDb;

        protected InternalDocumentSendLists DocSendList;

        public DeleteDocumentSendListCommand(IDocumentOperationsDbProcess operationDb, IAdminsDbProcess adminDb)
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
            _context.SetCurrentPosition(_document.ExecutorPositionId);
            _adminDb.VerifyAccess(_context, new VerifyAccess { DocumentActionCode = CommandType.ToString() });

            DocSendList = _operationDb.DeleteDocumentSendListPrepare(_context, Model);

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

        public override EnumDocumentAdditionActions CommandType => EnumDocumentAdditionActions.DeleteDocumentSendList;
    }
}