using BL.Database.Documents.Interfaces;
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
    public class AddDocumentSendListCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminsDbProcess _adminDb;

        protected InternalDocumentSendList DocSendList;

        public AddDocumentSendListCommand(IDocumentOperationsDbProcess operationDb, IAdminsDbProcess adminDb)
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

            _adminDb.VerifyAccess(_context, CommandType);
            _document = _operationDb.ChangeDocumentSendListPrepare(_context, Model.DocumentId);
            Model.IsInitial = !_document.IsLaunchPlan;

            DocSendList = CommonDocumentUtilities.GetNewDocumentSendList(_context, Model);
            _document.SendLists.ToList().Add(DocSendList);

            CommonDocumentUtilities.VerifySendLists(_document);

            return true;
        }

        public override object Execute()
        {
            _operationDb.AddDocumentSendList(_context, new List<InternalDocumentSendList> { DocSendList });
            return null;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.AddDocumentSendList;
    }
}