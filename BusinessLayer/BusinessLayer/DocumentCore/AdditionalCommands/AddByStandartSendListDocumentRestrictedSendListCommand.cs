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
    public class AddByStandartSendListDocumentRestrictedSendListCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminService _admin;

        protected IEnumerable<InternalDocumentRestrictedSendList> DocRestSendLists;

        public AddByStandartSendListDocumentRestrictedSendListCommand(IDocumentOperationsDbProcess operationDb, IAdminService admin)
        {
            _admin = admin;
            _operationDb = operationDb;
        }

        private ModifyDocumentRestrictedSendListByStandartSendList Model
        {
            get
            {
                if (!(_param is ModifyDocumentRestrictedSendListByStandartSendList))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDocumentRestrictedSendListByStandartSendList)_param;
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

            DocRestSendLists = _operationDb.AddByStandartSendListDocumentRestrictedSendListPrepare(_context, Model);

            _document.RestrictedSendLists.ToList().AddRange(DocRestSendLists);

            CommonDocumentUtilities.VerifySendLists(_document);

            return true;
        }

        public override object Execute()
        {
            foreach(var rsl in DocRestSendLists)
            {
                CommonDocumentUtilities.SetLastChange(_context, rsl);
            }
            _operationDb.AddDocumentRestrictedSendList(_context, DocRestSendLists);
            return null;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.AddByStandartSendListDocumentRestrictedSendList;
    }
}