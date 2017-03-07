using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BL.Logic.DictionaryCore
{
    public class BaseUserPositionExecutorCommand : BaseDictionaryCommand
    {
        private AddPositionExecutor Model { get { return GetModel<AddPositionExecutor>(); } }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            // делегатами могут быть толь ио или референты
            if (Model.PositionExecutorTypeId == EnumPositionExecutionTypes.Personal) throw new UserPositionExecutorIsIncorrect();

            // назначать самого себя нельзя
            if (Model.AgentId == _context.CurrentAgentId) throw new UserPositionExecutorIsIncorrect();

            // делегировать можно только ту должность на которой назначен штатно
            var positionExecutor = _dictDb.GetInternalPositionExecutors(_context, new FilterDictionaryPositionExecutor
            {
                IsActive = true,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow,
                PositionIDs = new List<int> { (int)Model.PositionId },
                AgentIDs = new List<int> { _context.CurrentAgentId },
                PositionExecutorTypeIDs = new List<EnumPositionExecutionTypes> { EnumPositionExecutionTypes.Personal }
            }).FirstOrDefault();

            if (positionExecutor == null) throw new UserPositionExecutorIsIncorrect();

            return true;
        }

        public override object Execute()
        { throw new NotImplementedException(); }
    }
}