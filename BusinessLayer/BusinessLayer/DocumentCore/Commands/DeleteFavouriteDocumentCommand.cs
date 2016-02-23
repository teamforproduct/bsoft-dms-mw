using System;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Database.Admins.Interfaces;
using BL.Model.AdminCore;
using BL.Model.DocumentCore.InternalModel;

namespace BL.Logic.DocumentCore.Commands
{
    public class DeleteFavouriteDocumentCommand: BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminsDbProcess _adminDb;

        protected InternalDocumentAccesses DocAccess;

        public DeleteFavouriteDocumentCommand(IDocumentOperationsDbProcess operationDb, IAdminsDbProcess adminDb)
        {
            _operationDb = operationDb;
            _adminDb = adminDb;
        }

        private ChangeFavourites Model
        {
            get
            {
                if (!(_param is ChangeFavourites))
                {
                    throw new WrongParameterTypeError();
                }
                return (ChangeFavourites) _param;
            }
        }

        public override bool CanBeDisplayed()
        {
            return true;
        }

        public override bool CanExecute()
        {
            _adminDb.VerifyAccess(_context, new VerifyAccess { DocumentActionCode = CommandType.ToString() });
            DocAccess = _operationDb.ChangeIsFavouriteAccessPrepare(_context, Model.DocumentId);
            if (DocAccess == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            if (!DocAccess.IsFavourite)
            {
                throw new CouldNotChangeFavourite();
            }
            return true;
        }

        public override object Execute()
        {
            DocAccess.IsFavourite = false;
            DocAccess.LastChangeDate = DateTime.Now;
            DocAccess.LastChangeUserId = _context.CurrentAgentId;
            _operationDb.ChangeIsFavouriteAccess(_context, DocAccess);
            return null;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.DeleteFavourite;
    }
}