﻿using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Logic.Common;
using System.Linq;
using BL.Model.DocumentCore.InternalModel;

namespace BL.Logic.DocumentCore.AdditionalCommands
{
    public class DeleteDocumentLinkCommand: BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        public DeleteDocumentLinkCommand(IDocumentOperationsDbProcess operationDb)
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
                return (int) _param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            if ((_document.Accesses?.Count() ?? 0) != 0 && !_document.Accesses.Any(x => x.PositionId == positionId && x.IsInWork))
                return false;
            _actionRecords =
                _document.Links.Where(
                    x =>
                        x.ExecutorPositionId == positionId)
                        .Select(x => new InternalActionRecord
                        {
                            LinkId = x.Id
                        });
            if (!_actionRecords.Any())
            {
                return false;
            }

            return true;
        }

        public override bool CanExecute()
        {
            _document = _operationDb.DeleteDocumentLinkPrepare(_context, Model);
            var link = _document.Links.FirstOrDefault();
            if (_document?.Id == null || _document?.LinkId == null || link == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            _context.SetCurrentPosition(link.ExecutorPositionId);
            _admin.VerifyAccess(_context, CommandType);
            if (!CanBeDisplayed(_context.CurrentPositionId))
            {
                throw new CouldNotPerformOperation();
            }
            return true;
        }

        public override object Execute()
        {
            if (_document.LinkedDocumentsCount<2)
            {
                _document.NewLinkId = null;
            }
            CommonDocumentUtilities.SetLastChange(_context, _document);
            _document.Events = CommonDocumentUtilities.GetNewDocumentEvents(_context, _document.Id, EnumEventTypes.DeleteLink);
            _operationDb.DeleteDocumentLink(_context, _document);
            return null;
        }

    }
}