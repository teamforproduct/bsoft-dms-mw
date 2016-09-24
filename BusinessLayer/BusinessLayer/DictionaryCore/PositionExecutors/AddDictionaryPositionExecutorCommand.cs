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
            _adminService.VerifyAccess(_context, CommandType, false);

            DictionaryModelVerifying.VerifyPositionExecutor(_context, _dictDb, Model);

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