using BL.Logic.Common;
using BL.Model.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryPositionExecutorCommand : BaseDictionaryPositionExecutorCommand
    {
        public override object Execute()
        {
            try
            {
                var dp = CommonDictionaryUtilities.PositionExecutorModifyToInternal(_context, Model);

                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {

                    _dictDb.UpdateExecutor(_context, dp);
                    var frontObj = _dictDb.GetPositionExecutors(_context, new FilterDictionaryPositionExecutor { IDs = new List<int> { dp.Id } }).FirstOrDefault();
                    _logger.Information(_context, null, (int)EnumObjects.DictionaryPositionExecutors, (int)CommandType, frontObj);

                    // Синхронизация параметров в UserRoles:
                    _adminDb.UpdateUserRolePeriod(_context, new BL.Model.DictionaryCore.InternalModel.InternalDictionaryPositionExecutor(Model));
                    transaction.Complete();
                }

                return Model.Id;
            }
            catch (DictionaryRecordWasNotFound)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseError(ex);
            }
        }
    }
}