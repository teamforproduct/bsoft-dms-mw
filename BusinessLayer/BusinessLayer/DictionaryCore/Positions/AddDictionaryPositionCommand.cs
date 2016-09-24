using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;

namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryPositionCommand : BaseDictionaryCommand
    {

        private ModifyDictionaryPosition Model
        {
            get
            {
                if (!(_param is ModifyDictionaryPosition))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDictionaryPosition)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {

            _adminService.VerifyAccess(_context, CommandType, false);

            DictionaryModelVerifying.VerifyPosition(_context, _dictDb, Model);

            return true;
        }

        public override object Execute()
        {
            try
            {
                var dp = CommonDictionaryUtilities.PositionModifyToInternal(_context, Model);

                var positionId = _dictDb.AddPosition(_context, dp);

                // Если order не задан, задаю максимальный.
                if (Model.Order < 1)
                { _dictService.SetPositionOrder(_context, positionId, 100000000); }

                return positionId;
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}