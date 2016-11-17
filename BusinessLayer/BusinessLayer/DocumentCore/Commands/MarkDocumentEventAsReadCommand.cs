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

        private IEnumerable<InternalDocumentEvent> _events { get; set; }

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
            _events = _documentDb.MarkDocumentEventsAsReadPrepare(_context, Model);
            if (!_events.Any())
            {
                return false;
            }
            return true;
        }

        public override object Execute()
        {
            foreach (var x in _events)
                {
                    x.LastChangeUserId = _context.CurrentAgentId;
                    x.LastChangeDate = DateTime.UtcNow;
                    x.ReadDate = DateTime.UtcNow;
                    x.ReadAgentId = _context.CurrentAgentId;
                }
            _documentDb.MarkDocumentEventAsRead(_context, _events);
            return null;
        }
    }
}