using System.Collections.Generic;
using System.Linq;
using BL.Database.Admins.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.DocumentCore.IncomingModel;
using System.Linq;
using BL.Logic.Common;
using System.Collections.Generic;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.Enums;
using BL.Model.SystemCore;

namespace BL.Logic.DocumentCore.SendListCommands
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

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _document = _operationDb.ChangeDocumentSendListPrepare(_context, Model.DocumentId);

            _context.SetCurrentPosition(_document.ExecutorPositionId);
            _admin.VerifyAccess(_context, CommandType);

            DocRestSendLists = _operationDb.AddByStandartSendListDocumentRestrictedSendListPrepare(_context, Model);

            var restrictedSendLists = _document.RestrictedSendLists.ToList();
            restrictedSendLists.AddRange(DocRestSendLists);
            _document.RestrictedSendLists = restrictedSendLists;

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