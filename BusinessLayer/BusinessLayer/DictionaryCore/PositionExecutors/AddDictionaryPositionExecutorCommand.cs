using BL.Logic.Common;
using BL.Model.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;

namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryPositionExecutorCommand : BaseDictionaryCommand
    {

        private ModifyDictionaryPositionExecutor Model
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

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {

            _admin.VerifyAccess(_context, CommandType, false);

            var fd = new FilterDictionaryPositionExecutor {
                PositionIDs = new List<int> { Model.PositionId },
                AgentIDs = new List<int> { Model.AgentId },
                Period = new Period(Model.StartDate, Model.EndDate),
                AccessLevelIDs = new List<int> { Model.AgentId },
                PositionExecutorTypeIDs = new List < int > { Model.PositionExecutorTypeId }
            };

            

            if (_dictDb.ExistsPositionExecutor(_context, fd))
            {
                throw new DictionaryRecordNotUnique();
            }

            return true;
        }

        public override object Execute()
        {
            try
            {
                var dp = CommonDictionaryUtilities.PositionExecutorModifyToInternal(_context, Model);

                return _dictDb.AddExecutor(_context, dp);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}