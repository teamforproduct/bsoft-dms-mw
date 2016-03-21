using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Context;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.SendListCommands
{
    public class AddByStandartSendListDocumentSendListCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        protected IEnumerable<InternalDocumentSendList> DocSendLists;

        public AddByStandartSendListDocumentSendListCommand(IDocumentOperationsDbProcess operationDb)
        {
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

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {

            _admin.VerifyAccess(_context, CommandType);

            _document = _operationDb.ChangeDocumentSendListPrepare(_context, Model.DocumentId);            
            Model.IsInitial = (_context is AdminContext) || _context.CurrentPositionsIdList.Contains(_document.ExecutorPositionId);

            DocSendLists = _operationDb.AddByStandartSendListDocumentSendListPrepare(_context, Model);  //TODO так нельзя!!!

            var sendLists = _document.SendLists.ToList();
            sendLists.AddRange(DocSendLists);
            _document.SendLists = sendLists;

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