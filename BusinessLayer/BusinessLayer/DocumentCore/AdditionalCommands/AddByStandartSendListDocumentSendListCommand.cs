using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Exception;
using BL.Database.Admins.Interfaces;
using BL.Model.AdminCore;
using BL.Model.DocumentCore.IncomingModel;
using System.Linq;
using BL.Logic.Common;
using System.Collections.Generic;
using BL.Model.Enums;

namespace BL.Logic.DocumentCore.AdditionalCommands
{
    public class AddByStandartSendListDocumentSendListCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminsDbProcess _adminDb;

        protected IEnumerable<InternalDocumentSendList> DocSendLists;

        public AddByStandartSendListDocumentSendListCommand(IDocumentOperationsDbProcess operationDb, IAdminsDbProcess adminDb)
        {
            _adminDb = adminDb;
            _operationDb = operationDb;
        }

        private ModifyDocumentSendListByStandartSendList Model
        {
            get
            {
                if (!(_param is ModifyDocumentSendListByStandartSendList))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDocumentSendListByStandartSendList)_param;
            }
        }

        public override bool CanBeDisplayed()
        {
            return true;
        }

        public override bool CanExecute()
        {

            _adminDb.VerifyAccess(_context, CommandType);

            _document = _operationDb.ChangeDocumentSendListPrepare(_context, Model.DocumentId);
            Model.IsInitial = !_document.IsLaunchPlan;

            DocSendLists = _operationDb.AddByStandartSendListDocumentSendListPrepare(_context, Model);

            _document.SendLists.ToList().AddRange(DocSendLists);

            CommonDocumentUtilities.VerifySendLists(_document);

            return true;
        }

        public override object Execute()
        {
            _operationDb.AddDocumentSendList(_context, DocSendLists);
            return null;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.AddByStandartSendListDocumentSendList;
    }
}