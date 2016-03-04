﻿using System.Linq;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.SystemCore;

namespace BL.Logic.DocumentCore.Commands
{
    public class AddFavouriteDocumentCommand: BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminService _admin;

        private InternalDocumentAccess _docAccess;

        public AddFavouriteDocumentCommand(IDocumentOperationsDbProcess operationDb, IAdminService admin)
        {
            _operationDb = operationDb;
            _admin = admin;
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

        public override bool CanBeDisplayed(int positionId)
        {
            if (!_document.Accesses.Any(x=>x.PositionId == positionId && !x.IsFavourite)
                )
            {
                return false;
            }

            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType);
            _document = _operationDb.ChangeIsFavouriteAccessPrepare(_context, Model.DocumentId);
            _docAccess = _document?.Accesses.FirstOrDefault();
            if (_docAccess == null
                || !CanBeDisplayed(_docAccess.PositionId)
                )
            {
                throw new CouldNotPerformThisOperation();
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

    }
}