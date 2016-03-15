using System;
using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.Commands
{
    public class MarkDocumentEventAsReadCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _documentDb;

        public MarkDocumentEventAsReadCommand(IDocumentOperationsDbProcess documentDb)
        {
            _documentDb = documentDb;
        }

        private int Model
        {
            get
            {
                if (!(_param is int))
                {
                    throw new WrongParameterTypeError();
                }
                return (int)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return false;
        }

        public override bool CanExecute()
        {
            _document = _documentDb.MarkDocumentEventsAsReadPrepare(_context, Model);
            if (!_document.Events.Any())
            {
                return false;
            }
            return true;
        }

        public override object Execute()
        {
            foreach (var x in _document.Events)
                {
                    x.LastChangeUserId = _context.CurrentAgentId;
                    x.LastChangeDate = DateTime.Now;
                    x.ReadDate = DateTime.Now;
                    x.ReadAgentId = _context.CurrentAgentId;
                }
            _documentDb.MarkDocumentEventAsRead(_context, _document.Events);
            return null;
        }
    }
}