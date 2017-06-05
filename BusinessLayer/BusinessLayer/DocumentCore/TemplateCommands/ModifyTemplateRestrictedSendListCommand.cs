
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.Common;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Exception;
using System.Collections.Generic;

namespace BL.Logic.DocumentCore.TemplateCommands
{
    public class ModifyTemplateRestrictedSendListCommand : BaseDocumentCommand
    {
        private readonly ITemplateDbProcess _operationDb;

        public ModifyTemplateRestrictedSendListCommand(ITemplateDbProcess operationDb)
        {
            _operationDb = operationDb;

        }

        private ModifyTemplateRestrictedSendList Model
        {
            get
            {
                if (!(_param is ModifyTemplateRestrictedSendList))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyTemplateRestrictedSendList)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _adminProc.VerifyAccess(_context, CommandType, false);
            if (_operationDb.ExistsTemplateRestrictedSendLists(_context, new FilterTemplateRestrictedSendList { TemplateId = Model.DocumentId, PositionId = Model.PositionId, NotContainsIDs = new List<int> { Model.Id} }))
            {
                throw new RecordNotUnique();
            }
            return true;
        }

        public override object Execute()
        {
            var model = new InternalTemplateRestrictedSendList(Model);
            CommonDocumentUtilities.SetLastChange(_context, model);
            return _operationDb.AddOrUpdateTemplateRestrictedSendList(_context, model);
        }

    }
}