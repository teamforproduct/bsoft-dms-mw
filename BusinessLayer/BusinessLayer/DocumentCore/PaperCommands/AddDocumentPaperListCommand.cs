using System;
using System.Collections.Generic;
using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.PaperCommands
{
    public class AddDocumentPaperListCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        InternalDocumentPaperList _paperList;

        public AddDocumentPaperListCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
        }

        private AddDocumentPaperList Model
        {
            get
            {
                if (!(_param is AddDocumentPaperList))
                {
                    throw new WrongParameterTypeError();
                }
                return (AddDocumentPaperList)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _paperList = _operationDb.AddDocumentPaperListsPrepare(_context, Model);
            if (!_paperList.Events.Any())
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }

            _adminProc.VerifyAccess(_context, CommandType,false);

            return true;
        }

        public override object Execute()
        {
            var _paperLists = _paperList.Events
                .GroupBy(x => new { x.SourcePositionId, x.TargetPositionId, x.TargetAgentId })
                .Select(x => new InternalDocumentPaperList
                {
                    Date = DateTime.UtcNow,
                    Description = Model.Description,
                    Events = x.Select(y => new InternalDocumentEvent
                    {
                        Id = y.Id,
                        ClientId = y.ClientId,
                        EntityTypeId = y.EntityTypeId,
                        LastChangeDate = DateTime.UtcNow,
                        LastChangeUserId = _context.CurrentAgentId,
                    }).ToList()
                }).ToList();
            CommonDocumentUtilities.SetLastChange(_context, _paperLists);
            return _operationDb.AddDocumentPaperLists(_context, _paperLists).ToList();

        }

    }
}