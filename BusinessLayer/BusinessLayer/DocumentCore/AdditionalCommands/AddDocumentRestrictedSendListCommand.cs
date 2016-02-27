using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DocumentCore.IncomingModel;
using System.Linq;
using BL.Logic.Common;
using System.Collections.Generic;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.Enums;

namespace BL.Logic.DocumentCore.AdditionalCommands
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

        public override bool CanBeDisplayed()
        {
            return true;
        }

        public override bool CanExecute()
        {
            _context.SetCurrentPosition(_document.ExecutorPositionId);
            _admin.VerifyAccess(_context, CommandType);

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