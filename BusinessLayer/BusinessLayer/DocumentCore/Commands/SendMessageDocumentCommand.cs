using System.Collections.Generic;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.Enums;
using BL.Model.Exception;
using System.Linq;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.DocumentCore.InternalModel;

namespace BL.Logic.DocumentCore.Commands
{
    public class SendMessageDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentsDbProcess _documentDb;
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminService _admin;

        public SendMessageDocumentCommand(IDocumentsDbProcess documentDb, IDocumentOperationsDbProcess operationDb, IAdminService admin)
        {
            _documentDb = documentDb;
            _operationDb = operationDb;
            _admin = admin;
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
            _admin.VerifyAccess(_context, CommandType);
            _document = _documentDb.GetBlankInternalDocumentById(_context, Model.DocumentId);

            return true;
        }

        public override object Execute()
        {
            var evtToAdd = new List<InternalDocumentEvent>();
            if (Model?.Positions.Count == 0)
            {
                var evt = CommonDocumentUtilities.GetNewDocumentEvent(_context, Model.DocumentId,
                    EnumEventTypes.SendMessage, Model.Description, null);
                evt.TargetPositionId = null;
                evtToAdd.Add(evt);
            }
            else
            {
                var accList = _operationDb.GetDocumentAccesses(_context, Model.DocumentId);
                var actuelPosList = Model.Positions.Where(x => accList.Select(s => s.PositionId).Contains(x)).ToList();
                if (!actuelPosList.Any()) return null;

                var posInfos = _operationDb.GetInternalPositionsInfo(_context, actuelPosList);

                var description = Model.Description + (
                    Model.IsAddPositionsInfo
                        ? "[" + string.Join(", ", posInfos.Select(x => x.PositionName)) + "]"
                        : "");

                evtToAdd.AddRange(actuelPosList.Select(targetPositionId => 
                            CommonDocumentUtilities.GetNewDocumentEvent(_context, Model.DocumentId, EnumEventTypes.SendMessage, description, null, targetPositionId)));
            }
            _operationDb.AddDocumentEvents(_context, evtToAdd);

            return null;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.SendMessage;
    }
}