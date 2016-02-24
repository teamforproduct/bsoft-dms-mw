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
    public class AddDocumentRestrictedSendListCommand : BaseDocumentAdditionCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminsDbProcess _adminDb;

        protected InternalDocumentRestrictedSendLists DocRestSendList;

        public AddDocumentRestrictedSendListCommand(IDocumentOperationsDbProcess operationDb, IAdminsDbProcess adminDb)
        {
            _adminDb = adminDb;
            _operationDb = operationDb;
        }

        private ModifyDocumentRestrictedSendList Model
        {
            get
            {
                if (!(_param is ModifyDocumentRestrictedSendList))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDocumentRestrictedSendList)_param;
            }
        }

        public override bool CanBeDisplayed()
        {
            _context.SetCurrentPosition(_document.ExecutorPositionId);
            _adminDb.VerifyAccess(_context, new VerifyAccess { DocumentActionCode = CommandType.ToString() });
            return true;
        }

        public override bool CanExecute()
        {
            _document = _operationDb.ChangeDocumentSendListPrepare(_context, Model.DocumentId);

            CanBeDisplayed();

            DocRestSendList = new InternalDocumentRestrictedSendLists
            {
                AccessLevel = Model.AccessLevel,
                DocumentId = Model.DocumentId,
                PositionId = Model.PositionId
            };

            _document.RestrictedSendLists.ToList().Add(DocRestSendList);

            CommonDocumentUtilities.VerifySendLists(_document);

            return true;
        }

        public override object Execute()
        {
            DocRestSendList.LastChangeUserId = _context.CurrentAgentId;
            DocRestSendList.LastChangeDate = DateTime.Now;
            _operationDb.AddDocumentRestrictedSendList(_context, new List<InternalDocumentRestrictedSendLists> { DocRestSendList });
            return null;
        }

        public override EnumDocumentAdditionActions CommandType => EnumDocumentAdditionActions.AddDocumentRestrictedSendList;
    }
}