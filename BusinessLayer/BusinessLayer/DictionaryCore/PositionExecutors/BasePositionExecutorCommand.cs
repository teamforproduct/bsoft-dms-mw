using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BL.Logic.DictionaryCore
{
    public class BasePositionExecutorCommand : BaseDictionaryCommand
    {
        private AddPositionExecutor Model { get { return GetModel<AddPositionExecutor>(); } }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            if (Model.StartDate > Model.EndDate) throw new DictionaryPositionExecutorIsInvalidPeriod();

            FrontDictionaryPositionExecutor executor = null;

            var filter = new FilterDictionaryPositionExecutor
            {
                PositionIDs = new List<int> { Model.PositionId },
                StartDate = Model.StartDate,
                EndDate = Model.EndDate,
                AgentIDs = new List<int> { Model.AgentId },
            };

            if (TypeModelIs<ModifyPositionExecutor>())
            { filter.NotContainsIDs = new List<int> { GetModel<ModifyPositionExecutor>().Id }; }

            var PositionExecutors = _dictDb.GetPositionExecutors(_context, filter);

            executor = PositionExecutors.FirstOrDefault();

            if (executor != null)
            { throw new DictionaryPositionExecutorNotUnique(executor.PositionName, executor.AgentName, executor.StartDate.ToString("dd.MM.yyyy HH:mm"), executor.EndDate.HasValue ? executor.EndDate.Value.ToString("dd.MM.yyyy HH:mm") : "-"); }



            switch (Model.PositionExecutorTypeId)
            {
                case EnumPositionExecutionTypes.Personal:

                    filter = new FilterDictionaryPositionExecutor
                    {
                        PositionIDs = new List<int> { Model.PositionId },
                        StartDate = Model.StartDate,
                        EndDate = Model.EndDate,
                        PositionExecutorTypeIDs = new List<EnumPositionExecutionTypes> { Model.PositionExecutorTypeId },
                    };

                    if (TypeModelIs<ModifyPositionExecutor>())
                    { filter.NotContainsIDs = new List<int> { GetModel<ModifyPositionExecutor>().Id }; }

                    // Personal может быть только один на должности за период
                    executor = _dictDb.GetPositionExecutors(_context, filter).FirstOrDefault();

                    if (executor != null)
                    { throw new DictionaryPositionExecutorPersonalNotUnique(executor.PositionName, executor.AgentName, executor.StartDate.ToString("dd.MM.yyyy HH:mm"), executor.EndDate.HasValue ? executor.EndDate.Value.ToString("dd.MM.yyyy HH:mm") : "-"); }
                    break;
                case EnumPositionExecutionTypes.IO:
                    filter = new FilterDictionaryPositionExecutor
                    {
                        PositionIDs = new List<int> { Model.PositionId },
                        StartDate = Model.StartDate,
                        EndDate = Model.EndDate,
                        PositionExecutorTypeIDs = new List<EnumPositionExecutionTypes> { Model.PositionExecutorTypeId },
                    };

                    if (TypeModelIs<ModifyPositionExecutor>())
                    { filter.NotContainsIDs = new List<int> { GetModel<ModifyPositionExecutor>().Id }; }

                    // IO может быть только один на должности за период
                    executor = _dictDb.GetPositionExecutors(_context, filter).FirstOrDefault();

                    if (executor != null)
                    { throw new DictionaryPositionExecutorIONotUnique(executor.PositionName, executor.AgentName, executor.StartDate.ToString("dd.MM.yyyy HH:mm"), executor.EndDate.HasValue ? executor.EndDate.Value.ToString("dd.MM.yyyy HH:mm") : "-"); }
                    break;
                case EnumPositionExecutionTypes.Referent:
                    filter = new FilterDictionaryPositionExecutor
                    {
                        PositionIDs = new List<int> { Model.PositionId },
                        StartDate = Model.StartDate,
                        EndDate = Model.EndDate,
                        PositionExecutorTypeIDs = new List<EnumPositionExecutionTypes> { Model.PositionExecutorTypeId },
                        AgentIDs = new List<int> { Model.AgentId },
                    };

                    if (TypeModelIs<ModifyPositionExecutor>())
                    { filter.NotContainsIDs = new List<int> { GetModel<ModifyPositionExecutor>().Id }; }

                    // Референтов может быть несколько может быть на должности за период
                    executor = _dictDb.GetPositionExecutors(_context, filter).FirstOrDefault();

                    if (executor != null)
                    { throw new DictionaryPositionExecutorReferentNotUnique(executor.PositionName, executor.AgentName, executor.StartDate.ToString("dd.MM.yyyy HH:mm"), executor.EndDate.HasValue ? executor.EndDate.Value.ToString("dd.MM.yyyy HH:mm") : "-"); }

                    break;
                default:

                    filter = new FilterDictionaryPositionExecutor
                    {
                        PositionIDs = new List<int> { Model.PositionId },
                        StartDate = Model.StartDate,
                        EndDate = Model.EndDate,
                        PositionExecutorTypeIDs = new List<EnumPositionExecutionTypes> { Model.PositionExecutorTypeId },
                        AgentIDs = new List<int> { Model.AgentId },
                    };

                    if (TypeModelIs<ModifyPositionExecutor>())
                    { filter.NotContainsIDs = new List<int> { GetModel<ModifyPositionExecutor>().Id }; }

                    executor = _dictDb.GetPositionExecutors(_context, filter).FirstOrDefault();

                    if (executor != null)
                    { throw new DictionaryPositionExecutorReferentNotUnique(executor.PositionName, executor.AgentName, executor.StartDate.ToString("dd.MM.yyyy HH:mm"), executor.EndDate.HasValue ? executor.EndDate.Value.ToString("dd.MM.yyyy HH:mm") : "-"); }

                    break;
            }

            return true;
        }

        public override object Execute()
        { throw new NotImplementedException(); }

    }
}