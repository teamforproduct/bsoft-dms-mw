using System;
using BL.CrossCutting.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.Commands
{
    public class DeleteFavouriteDocumentCommand: BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        public DeleteFavouriteDocumentCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
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
            return true;
        }

        public override object Execute()
        {
            var acc = _operationDb.GetDocumentAccess(_context, Model.DocumentId);
            acc.IsFavourite = false;
            acc.LastChangeDate = DateTime.Now;
            acc.LastChangeUserId = _context.CurrentAgentId;
            _operationDb.UpdateDocumentAccess(_context, acc);
            return null;
        }

        public override EnumDocumentActions CommandType { get { return EnumDocumentActions.DeleteFavourite; } }
    }
}