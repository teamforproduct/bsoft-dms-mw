using System.Collections.Generic;
using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.SystemCore;

namespace BL.Logic.DocumentCore.SendListCommands
{
    public class AddDocumentRestrictedSendListCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminService _admin;

        protected InternalDocumentRestrictedSendList DocRestSendList;

        public AddDocumentRestrictedSendListCommand(IDocumentOperationsDbProcess operationDb, IAdminService admin)
        {
            _admin = admin;
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

        public override bool CanBeDisplayed(int positionId, InternalSystemAction action)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _document = _operationDb.ChangeDocumentSendListPrepare(_context, Model.DocumentId);

            _context.SetCurrentPosition(_document.ExecutorPositionId);
            _admin.VerifyAccess(_context, CommandType);

            DocRestSendList = new InternalDocumentRestrictedSendList
            {
                AccessLevel = Model.AccessLevel,
                DocumentId = Model.DocumentId,
                PositionId = Model.PositionId
            };

            var restrictedSendLists = _document.RestrictedSendLists.ToList();
            restrictedSendLists.Add(DocRestSendList);
            _document.RestrictedSendLists = restrictedSendLists;

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