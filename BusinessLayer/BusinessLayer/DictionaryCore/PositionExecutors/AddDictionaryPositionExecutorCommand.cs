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
    public class AddDictionaryPositionExecutorCommand : BaseDictionaryPositionExecutorCommand
    {
        public override object Execute()
        {
            try
            {
                var dp = CommonDictionaryUtilities.PositionExecutorModifyToInternal(_context, Model);

                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {

                    Model.Id = _dictDb.AddExecutor(_context, dp);
                    var frontObj = _dictDb.GetDepartments(_context, new FilterDictionaryDepartment { IDs = new List<int> { dp.Id } }).FirstOrDefault();
                    _logger.Information(_context, null, (int)EnumObjects.DictionaryPositionExecutors, (int)CommandType, frontObj);

                    // При назначении сотрудника добавляю все роли должности
                    _adminService.AddAllPositionRoleForUser(_context, Model);
                    transaction.Complete();
                }

                return Model.Id;
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}