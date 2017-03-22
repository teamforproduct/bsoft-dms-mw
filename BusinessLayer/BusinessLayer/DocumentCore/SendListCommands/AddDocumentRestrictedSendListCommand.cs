using System.Collections.Generic;
using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.SendListCommands
{
    public class AddDocumentRestrictedSendListCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        protected InternalDocumentRestrictedSendList DocRestSendList;

        public AddDocumentRestrictedSendListCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
        }

        private ModifyDocumentRestrictedSendList Model
        {
            get
            {
                if (!(_param is ModifyDocumentRestrictedSendList))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDocumentRestrictedSendList)_param;
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

            DocRestSendList = CommonDocumentUtilities.GetNewDocumentRestrictedSendList(_context, (int)EnumEntytiTypes.Document, Model);

            var restrictedSendLists = _document.RestrictedSendLists.ToList();
            restrictedSendLists.Add(DocRestSendList);
            _document.RestrictedSendLists = restrictedSendLists;

            CommonDocumentUtilities.VerifySendLists(_document);

            return true;
        }

        public override object Execute()
        {
            return _operationDb.AddDocumentRestrictedSendList(_context, new List<InternalDocumentRestrictedSendList> { DocRestSendList }).FirstOrDefault();
        }

    }
}