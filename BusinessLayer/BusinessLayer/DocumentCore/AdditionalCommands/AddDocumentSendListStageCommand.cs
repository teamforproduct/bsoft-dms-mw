using BL.Database.Documents.Interfaces;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Database.Admins.Interfaces;
using BL.Model.AdminCore;
using BL.Model.DocumentCore.IncomingModel;
using System.Linq;
using BL.Logic.Common;

namespace BL.Logic.DocumentCore.AdditionalCommands
{
    public class AddDocumentSendListStageCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminsDbProcess _adminDb;

        public AddDocumentSendListStageCommand(IDocumentOperationsDbProcess operationDb, IAdminsDbProcess adminDb)
        {
            _adminDb = adminDb;
            _operationDb = operationDb;
        }

        private ModifyDocumentSendListStage Model
        {
            get
            {
                if (!(_param is ModifyDocumentSendListStage))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDocumentSendListStage)_param;
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

            _document = _operationDb.AddDocumentSendListStagePrepare(_context, Model.DocumentId);

            return true;
        }

        public override object Execute()
        {
            if (Model.Stage >= 0)
            {
                var sendLists = _document.SendLists.Where(x => x.Stage >= Model.Stage).ToList();

                if (sendLists.Any())
                {
                    foreach (var sl in sendLists)
                    {
                        sl.Stage++;
                        CommonDocumentUtilities.SetLastChange(_context, sl);
                    }
                    _operationDb.ChangeDocumentSendListStage(_context, sendLists);
                }
                else
                    return true;
            }
            return false;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.AddDocumentSendListStage;
    }
}