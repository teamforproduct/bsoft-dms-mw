using System;
using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.SystemCore;

namespace BL.Logic.DocumentCore.SendListCommands
{
    public class LaunchDocumentSendListItemCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminService _admin;

        private InternalDocumentSendList _sendList;

        public LaunchDocumentSendListItemCommand(IDocumentOperationsDbProcess operationDb, IAdminService admin)
        {
            _admin = admin;
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

        public override bool CanBeDisplayed(int positionId, InternalSystemAction action)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _document = _operationDb.LaunchDocumentSendListItemPrepare(_context, Model);
            if (_document?.SendLists == null || !_document.SendLists.Any())
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            _sendList = _document.SendLists.First();
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