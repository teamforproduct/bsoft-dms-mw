using BL.Logic.AdminCore;
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
    public class AddDictionaryPositionCommand : BaseDictionaryPositionCommand
    {
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
                    _logger.Information(_context, null, (int)EnumObjects.DictionaryPositions, (int)CommandType, frontObj.Id, frontObj);

                    _dictService.SetPositionOrder(_context, positionId, Model.Order);

                    transaction.Complete();
                }


                // Добавляю рассылку (subordinations). 
                // Если SUBORDINATIONS_SEND_ALL_FOR_EXECUTION и SUBORDINATIONS_SEND_ALL_FOR_EXECUTION включены, то разрешаю рассылку на всех

                // Включаю доступ к журналам в своем отделе



                return positionId;
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }

        private void SetDefaultRJournalPositions(SetDefaultRJournalPositionsCommand model)
        {
            _adminService.ExecuteAction(EnumAdminActions.SetRegistrationJournalPositionByDepartment, _context, model);
        }

    }
}