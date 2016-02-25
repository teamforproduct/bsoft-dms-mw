using System;
using System.Linq;
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

        private InternalDocumentAccess _docAccess;

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
            _adminDb.VerifyAccess(_context, CommandType);
            _document = _operationDb.ChangeIsFavouriteAccessPrepare(_context, Model.DocumentId);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            if (!_document.IsFavourite)
            {
                throw new CouldNotChangeFavourite();
            }
            _docAccess = _document.Accesses.FirstOrDefault();
            return true;
        }

        public override object Execute()
        {
            _docAccess.IsFavourite = false;
            CommonDocumentUtilities.SetLastChange(_context, _docAccess);
            _operationDb.ChangeIsFavouriteAccess(_context, _docAccess);
            return null;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.DeleteFavourite;
    }
}