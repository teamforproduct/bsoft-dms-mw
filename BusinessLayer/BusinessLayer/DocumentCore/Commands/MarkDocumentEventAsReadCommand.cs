using System;
using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.Exception;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using System.Collections.Generic;

namespace BL.Logic.DocumentCore.Commands
{
    public class MarkDocumentEventAsReadCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _documentDb;

        public MarkDocumentEventAsReadCommand(IDocumentOperationsDbProcess documentDb)
        {
            _documentDb = documentDb;
        }

        private IEnumerable<InternalDocumentEventAccess> _eventAccesses { get; set; }

        private MarkDocumentEventAsRead Model
        {
            get
            {
                if (!(_param is MarkDocumentEventAsRead))
                {
                    throw new WrongParameterTypeError();
                }
                return (MarkDocumentEventAsRead)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return false;
        }

        public override bool CanExecute()
        {
            _eventAccesses = _documentDb.MarkDocumentEventsAsReadPrepare(_context, Model);
            if (!_eventAccesses.Any())
            {
                return false;
            }
            return true;
        }

        public override object Execute()
        {
            CommonDocumentUtilities.SetLastChange(_context, _eventAccesses);
            _eventAccesses.ToList().ForEach(x =>
            {
                x.ReadDate = DateTime.UtcNow;
                x.ReadAgentId = _context.CurrentAgentId;
            });
            _documentDb.MarkDocumentEventAsRead(_context, _eventAccesses);
            return null;
        }
    }
}