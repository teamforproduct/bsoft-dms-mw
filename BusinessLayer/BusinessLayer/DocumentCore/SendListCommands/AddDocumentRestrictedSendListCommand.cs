using System.Collections.Generic;
using System.Linq;
using BL.Database.Admins.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.SendListCommands
{
    public class AddDocumentRestrictedSendListCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminsDbProcess _adminDb;

        protected InternalDocumentRestrictedSendList DocRestSendList;

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
            return true;
        }

        public override bool CanExecute()
        {
            _context.SetCurrentPosition(_document.ExecutorPositionId);
            _adminDb.VerifyAccess(_context, CommandType);

            _document = _operationDb.ChangeDocumentSendListPrepare(_context, Model.DocumentId);

            DocRestSendList = new InternalDocumentRestrictedSendList
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
            CommonDocumentUtilities.SetLastChange(_context, DocRestSendList);
            _operationDb.AddDocumentRestrictedSendList(_context, new List<InternalDocumentRestrictedSendList> { DocRestSendList });
            return null;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.AddDocumentRestrictedSendList;
    }
}