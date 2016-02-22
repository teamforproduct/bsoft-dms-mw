using System;
using System.Collections.Generic;
using BL.CrossCutting.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.Enums;
using BL.Model.Exception;
using System.Linq;
using BL.Model.DocumentCore.InternalModel;

namespace BL.Logic.DocumentCore.Commands
{
    public class SendMessageDocumentCommand: BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _documentDb;

        public SendMessageDocumentCommand(IDocumentOperationsDbProcess documentDb)
        {
            _documentDb = documentDb;
        }

        private SendMessage Model
        {
            get
            {
                if (!(_param is SendMessage))
                {
                    throw new WrongParameterTypeError();
                }
                return (SendMessage)_param;
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
            var accList = _documentDb.GetDocumentAccesses(_context, Model.DocumentId);
            var actuelPosList = Model.Positions.Where(x => accList.Select(s => s.PositionId).Contains(x)).ToList();
            if (actuelPosList.Any())
            {
                var posInfos = _documentDb.GetInternalPositionsInfo(_context, actuelPosList);
                var evtToAdd = new List<InternalDocumentEvents>();

                string descr = Model.Description + (
                    Model.IsAddPositionsInfo
                    ? "[" + String.Join(", ", posInfos.Select(x => x.PositionName)) + "]"
                    : "");

                foreach (var pos in actuelPosList)
                {
                    evtToAdd.Add(new InternalDocumentEvents
                    {
                        EventType = EnumEventTypes.SendMessage,
                        Description = descr,
                        LastChangeUserId = _context.CurrentAgentId,
                        SourceAgentId = _context.CurrentAgentId,
                        SourcePositionId = _context.CurrentPositionId,
                        TargetAgentId = posInfos.First(x => x.PositionId == pos).AgentId,
                        TargetPositionId = pos,
                        LastChangeDate = DateTime.Now,
                        Date = DateTime.Now,
                        CreateDate = DateTime.Now,
                    });
                }
                _documentDb.AddDocumentEvents(_context, evtToAdd);
            }
            return null;
        }

        public override EnumDocumentActions CommandType { get { return EnumDocumentActions.SendMessage; } }
    }
}