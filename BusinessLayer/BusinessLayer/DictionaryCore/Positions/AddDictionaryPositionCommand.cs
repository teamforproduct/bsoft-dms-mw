using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

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
                int positionId;
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {

                    positionId = _dictDb.AddPosition(_context, dp);
                    var frontObj = _dictDb.GetPositions(_context, new FilterDictionaryPosition { IDs = new List<int> { dp.Id } }).FirstOrDefault();
                    _logger.Information(_context, null, (int)EnumObjects.DictionaryPositions, (int)CommandType, frontObj);

                // Если order не задан, задаю максимальный.
                if (Model.Order < 1)
                { _dictService.SetPositionOrder(_context, positionId, Int32.MaxValue); }
                    transaction.Complete();
                }


                // Добавляю рассылку (subordinations). 
                // Если SUBORDINATIONS_SEND_ALL_FOR_EXECUTION и SUBORDINATIONS_SEND_ALL_FOR_EXECUTION включены, то разрешаю рассылку на всех
                


                return positionId;
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}