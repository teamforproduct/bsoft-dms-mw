using System;
using System.Linq;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.AdminCore;
using BL.Database.Admins.Interfaces;
using BL.Model.DocumentCore.InternalModel;

namespace BL.Logic.DocumentCore.Commands
{
    public class AddFavouriteDocumentCommand: BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminsDbProcess _adminDb;

        private InternalDocumentAccess _docAccess;

        public AddFavouriteDocumentCommand(IDocumentOperationsDbProcess operationDb, IAdminsDbProcess adminDb)
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
            if (_document?.Accesses == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            _docAccess = _document.Accesses.FirstOrDefault();
            if (_docAccess.IsFavourite)
            {
                throw new CouldNotChangeFavourite();
            }
            return true;
        }

        public override object Execute()
        {
            _docAccess.IsFavourite = true;
            CommonDocumentUtilities.SetLastChange(_context, _docAccess);
            _operationDb.ChangeIsFavouriteAccess(_context, _docAccess);
            return null;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.AddFavourite;
    }
}