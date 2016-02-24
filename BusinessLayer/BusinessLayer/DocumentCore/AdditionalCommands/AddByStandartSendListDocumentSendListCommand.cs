using System;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Database.Admins.Interfaces;
using BL.Model.AdminCore;
using BL.Model.DocumentCore.IncomingModel;
using System.Linq;
using BL.Logic.Common;
using System.Collections.Generic;
using BL.Logic.DependencyInjection;
using BL.Database.Dictionaries.Interfaces;

namespace BL.Logic.DocumentCore.AdditionalCommands
{
    public class AddByStandartSendListDocumentSendListCommand : BaseDocumentAdditionCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminsDbProcess _adminDb;

        protected IEnumerable<InternalDocumentSendLists> DocSendLists;

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
            _context.SetCurrentPosition(_document.ExecutorPositionId);
            _adminDb.VerifyAccess(_context, new VerifyAccess { DocumentActionCode = CommandType.ToString() });

            _document = _operationDb.ChangeDocumentSendListPrepare(_context, Model.DocumentId);

            DocSendLists = _operationDb.AddByStandartSendListDocumentSendListPrepare(_context, Model);

            _document.SendLists.ToList().AddRange(DocSendLists);

            CommonDocumentUtilities.VerifySendLists(_document);

            return true;
        }

        public override object Execute()
        {
            foreach(var rsl in DocSendLists)
            {
                CommonDocumentUtilities.SetLastChange(_context, rsl);
            }
            _operationDb.AddDocumentSendList(_context, DocSendLists);
            return null;
        }

        public override EnumDocumentAdditionActions CommandType => EnumDocumentAdditionActions.AddByStandartSendListDocumentSendList;
    }
}