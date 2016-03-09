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
    public class ModifyDocumentSendListCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminService _admin;

        protected InternalDocumentSendList DocSendList;

        public ModifyDocumentSendListCommand(IDocumentOperationsDbProcess operationDb, IAdminService admin)
        {
            _admin = admin;
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

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _document = _operationDb.ChangeDocumentSendListPrepare(_context, Model.DocumentId);

            DocSendList = _document.SendLists.FirstOrDefault(x => x.Id == Model.Id);

            _context.SetCurrentPosition(DocSendList.SourcePositionId);
            _admin.VerifyAccess(_context, CommandType);


            DocSendList.Stage = Model.Stage;
            DocSendList.SendType = Model.SendType;
            DocSendList.TargetPositionId = Model.TargetPositionId;
            DocSendList.Task = Model.Task;
            DocSendList.Description = Model.Description;
            DocSendList.DueDate = Model.DueDate;
            DocSendList.DueDay = Model.DueDay;
            DocSendList.AccessLevel = Model.AccessLevel;
            DocSendList.IsInitial = Model.IsInitial;

            CommonDocumentUtilities.VerifySendLists(_document);

            return true;
        }

        public override object Execute()
        {
            CommonDocumentUtilities.SetLastChange(_context, DocSendList);
            _operationDb.ModifyDocumentSendList(_context, DocSendList);
            return null;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.ModifyDocumentSendList;
    }
}