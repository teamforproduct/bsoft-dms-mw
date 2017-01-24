using BL.CrossCutting.Helpers;
using BL.Logic.Common;
using BL.Model.Common;
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
    public class AddDictionaryPositionExecutorCommand : BaseDictionaryPositionExecutorCommand
    {
        private AddPositionExecutor Model { get { return GetModel<AddPositionExecutor>(); } }

        public override object Execute()
        {
            try
            {
                var  model = new InternalDictionaryPositionExecutor(Model);

                CommonDocumentUtilities.SetLastChange(_context, model);

                using (var transaction = Transactions.GetTransaction())
                {

                    model.Id = _dictDb.AddExecutor(_context, model);

                    var frontObj = _dictDb.GetPositionExecutors(_context, new FilterDictionaryPositionExecutor { IDs = new List<int> { model.Id } }).FirstOrDefault();
                    _logger.Information(_context, null, (int)EnumObjects.DictionaryPositionExecutors, (int)CommandType, frontObj.Id, frontObj);

                    // При назначении сотрудника добавляю все роли должности
                    _adminService.AddAllPositionRoleForUser(_context, model);
                    transaction.Complete();

                }

                return model.Id;
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}