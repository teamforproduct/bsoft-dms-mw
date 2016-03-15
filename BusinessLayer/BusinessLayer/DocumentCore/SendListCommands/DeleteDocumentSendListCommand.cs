﻿using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.SendListCommands
{
    public class DeleteDocumentSendListCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        private InternalDocumentSendList _sendList;

        public DeleteDocumentSendListCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
        }

        private int Model
        {
            get
            {
                if (!(_param is int))
                {
                    throw new WrongParameterTypeError();
                }
                return (int)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            _actionRecords =
                _document.SendLists.Where(
                    x =>
                        x.SourcePositionId == positionId
                        && x.StartEventId == null && x.CloseEventId == null)
                                                .Select(x => new InternalActionRecord
                                                {
                                                    SendListId = x.Id,
                                                });
            if (!_actionRecords.Any())
            {
                return false;
            }
            return true;
        }

        public override bool CanExecute()
        {

            _document = _operationDb.DeleteDocumentSendListPrepare(_context, Model);
            _sendList = _document?.SendLists.FirstOrDefault(x => x.Id == Model);
            if (_sendList == null || !CanBeDisplayed(_sendList.SourcePositionId))
            {
                throw new CouldNotPerformThisOperation();
            }
            _context.SetCurrentPosition(_sendList.SourcePositionId);
            _admin.VerifyAccess(_context, CommandType);

            _document = _operationDb.ChangeDocumentSendListPrepare(_context, _sendList.DocumentId);

            var sendLists = _document.SendLists.ToList();
            sendLists.Remove(_document.SendLists.FirstOrDefault(x => x.Id == Model));
            _document.SendLists = sendLists;

            CommonDocumentUtilities.VerifySendLists(_document);

            return true;
        }

        public override object Execute()
        {
            _operationDb.DeleteDocumentSendList(_context, Model);
            return _sendList.DocumentId;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.DeleteDocumentSendList;
    }
}