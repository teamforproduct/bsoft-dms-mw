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
    public class ModifyDocumentSendListCommand : BaseDocumentAdditionCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminsDbProcess _adminDb;

        protected InternalDocumentSendLists DocSendList;

        public ModifyDocumentSendListCommand(IDocumentOperationsDbProcess operationDb, IAdminsDbProcess adminDb)
        {
            _adminDb = adminDb;
            _operationDb = operationDb;
        }

        private ModifyDocumentSendList Model
        {
            get
            {
                if (!(_param is ModifyDocumentSendList))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDocumentSendList)_param;
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

            DocSendList = _document.SendLists.FirstOrDefault(x => x.Id == Model.Id);
            DocSendList.Stage = Model.Stage;
            DocSendList.SendType = Model.SendType;
            DocSendList.TargetPositionId = Model.TargetPositionId;
            DocSendList.Description = Model.Description;
            DocSendList.DueDate = Model.DueDate;
            DocSendList.DueDay = Model.DueDay;
            DocSendList.AccessLevel = Model.AccessLevel;

            CommonDocumentUtilities.VerifySendLists(_document);

            return true;
        }

        public override object Execute()
        {
            CommonDocumentUtilities.SetLastChange(_context, DocSendList);
            _operationDb.ModifyDocumentSendList(_context, DocSendList);
            return null;
        }

        public override EnumDocumentAdditionActions CommandType => EnumDocumentAdditionActions.ModifyDocumentSendList;
    }
}