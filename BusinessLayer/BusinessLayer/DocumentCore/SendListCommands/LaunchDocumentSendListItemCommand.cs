using System;
using System.Linq;
using BL.CrossCutting.DependencyInjection;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Logic.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;

namespace BL.Logic.DocumentCore.SendListCommands
{
    public class LaunchDocumentSendListItemCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        private InternalDocumentSendList _sendList;

        public LaunchDocumentSendListItemCommand(IDocumentOperationsDbProcess operationDb)
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
            _document = _operationDb.LaunchDocumentSendListItemPrepare(_context, Model);
            _sendList = _document?.SendLists.FirstOrDefault();
            
            if (_sendList == null)
            {
                throw new CouldNotPerformOperation();
            }
            _sendList.SourcePositionId = _sendList.AccessGroups.Where(x => x.AccessType == EnumEventAccessTypes.Source).Select(x => x.PositionId).FirstOrDefault();
            if (!_sendList.SourcePositionId.HasValue || !CanBeDisplayed(_sendList.SourcePositionId.Value))
            {
                throw new CouldNotPerformOperation();
            }

            _context.SetCurrentPosition(_sendList.SourcePositionId);
            _admin.VerifyAccess(_context, CommandType);

            return true;
        }

        public override object Execute()
        {
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction((EnumDocumentActions)Enum.Parse(typeof(EnumDocumentActions), _sendList.SendType.ToString()), _context, _sendList);
            return _document.Id;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.LaunchDocumentSendListItem;
    }
}