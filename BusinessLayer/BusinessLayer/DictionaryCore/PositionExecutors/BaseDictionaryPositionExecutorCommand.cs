using BL.CrossCutting.Extensions;
using BL.Logic.Common;
using BL.Model.Common;
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
    public class BaseDictionaryPositionExecutorCommand : BaseDictionaryCommand
    {

        protected ModifyDictionaryPositionExecutor Model
        {
            get
            {
                if (!(_param is ModifyDictionaryPositionExecutor))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDictionaryPositionExecutor)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId) => true;

        public void PrepareData()
        {
            // Model.StartDate = Model.StartDate.StartOfDay();
            //Model.EndDate = Model.EndDate?.EndOfDay() ?? DateTime.MaxValue;


            if (Model.StartDate > Model.EndDate) throw new DictionaryPositionExecutorIsInvalidPeriod();
        }

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            PrepareData();

            FrontDictionaryPositionExecutor executor = null;


            var PositionExecutors = _dictDb.GetPositionExecutors(_context, new FilterDictionaryPositionExecutor
            {
                NotContainsIDs = new List<int> { Model.Id },
                PositionIDs = new List<int> { Model.PositionId },
                StartDate = Model.StartDate,
                EndDate = Model.EndDate,
                AgentIDs = new List<int> { Model.AgentId },
            });

            executor = PositionExecutors.FirstOrDefault();

            if (executor != null)
            { throw new DictionaryPositionExecutorNotUnique(executor.PositionName, executor.AgentName,  executor.StartDate.HasValue ? executor.StartDate.Value.ToString("dd.MM.yyyy HH:mm") : "-", executor.EndDate.HasValue ? executor.EndDate.Value.ToString("dd.MM.yyyy HH:mm") : "-"); }


            switch (Model.PositionExecutorTypeId)
            {
                case EnumPositionExecutionTypes.Personal:
                    // Personal может быть только один на должности за период
                    executor = _dictDb.GetPositionExecutors(_context, new FilterDictionaryPositionExecutor
                    {
                        NotContainsIDs = new List<int> { Model.Id },
                        PositionIDs = new List<int> { Model.PositionId },
                        StartDate = Model.StartDate,
                        EndDate = Model.EndDate,
                        PositionExecutorTypeIDs = new List<EnumPositionExecutionTypes> { Model.PositionExecutorTypeId },
                    }).FirstOrDefault();

                    if (executor != null)
                    { throw new DictionaryPositionExecutorPersonalNotUnique(executor.PositionName, executor.AgentName, executor.StartDate.HasValue ? executor.StartDate.Value.ToString("dd.MM.yyyy HH:mm") : "-", executor.EndDate.HasValue ? executor.EndDate.Value.ToString("dd.MM.yyyy HH:mm") : "-"); }
                    break;
                case EnumPositionExecutionTypes.IO:
                    // IO может быть только один на должности за период
                    executor = _dictDb.GetPositionExecutors(_context, new FilterDictionaryPositionExecutor
                    {
                        NotContainsIDs = new List<int> { Model.Id },
                        PositionIDs = new List<int> { Model.PositionId },
                        StartDate = Model.StartDate,
                        EndDate = Model.EndDate,
                        PositionExecutorTypeIDs = new List<EnumPositionExecutionTypes> { Model.PositionExecutorTypeId },
                    }).FirstOrDefault();

                    if (executor != null)
                    { throw new DictionaryPositionExecutorIONotUnique(executor.PositionName, executor.AgentName, executor.StartDate.HasValue ? executor.StartDate.Value.ToString("dd.MM.yyyy HH:mm") : "-", executor.EndDate.HasValue ? executor.EndDate.Value.ToString("dd.MM.yyyy HH:mm") : "-"); }
                    break;
                case EnumPositionExecutionTypes.Referent:
                    // Референтов может быть несколько может быть н на должности за период
                    executor = _dictDb.GetPositionExecutors(_context, new FilterDictionaryPositionExecutor
                    {
                        NotContainsIDs = new List<int> { Model.Id },
                        PositionIDs = new List<int> { Model.PositionId },
                        StartDate = Model.StartDate,
                        EndDate = Model.EndDate,
                        PositionExecutorTypeIDs = new List<EnumPositionExecutionTypes> { Model.PositionExecutorTypeId },
                        AgentIDs = new List<int> { Model.AgentId },
                    }).FirstOrDefault();

                    if (executor != null)
                    { throw new DictionaryPositionExecutorReferentNotUnique(executor.PositionName, executor.AgentName, executor.StartDate.HasValue ? executor.StartDate.Value.ToString("dd.MM.yyyy HH:mm") : "-", executor.EndDate.HasValue ? executor.EndDate.Value.ToString("dd.MM.yyyy HH:mm") : "-"); }

                    break;
                default:
                    executor = _dictDb.GetPositionExecutors(_context, new FilterDictionaryPositionExecutor
                    {
                        NotContainsIDs = new List<int> { Model.Id },
                        PositionIDs = new List<int> { Model.PositionId },
                        StartDate = Model.StartDate,
                        EndDate = Model.EndDate,
                        PositionExecutorTypeIDs = new List<EnumPositionExecutionTypes> { Model.PositionExecutorTypeId },
                        AgentIDs = new List<int> { Model.AgentId },
                    }).FirstOrDefault();

                    if (executor != null)
                    { throw new DictionaryPositionExecutorReferentNotUnique(executor.PositionName, executor.AgentName, executor.StartDate.HasValue ? executor.StartDate.Value.ToString("dd.MM.yyyy HH:mm") : "-", executor.EndDate.HasValue ? executor.EndDate.Value.ToString("dd.MM.yyyy HH:mm") : "-"); }

                    break;
            }

            return true;
        }

        public override object Execute()
        { throw new NotImplementedException(); }
    }
}