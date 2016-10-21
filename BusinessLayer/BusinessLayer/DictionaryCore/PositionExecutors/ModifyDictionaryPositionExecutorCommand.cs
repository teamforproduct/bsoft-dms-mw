using BL.Logic.Common;
using BL.Model.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;

namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryPositionExecutorCommand : BaseDictionaryPositionExecutorCommand
    {
        public override object Execute()
        {
            try
            {
                var dp = CommonDictionaryUtilities.PositionExecutorModifyToInternal(_context, Model);

                _dictDb.UpdateExecutor(_context, dp);

                // Синхронизация параметров в UserRoles:
                _adminDb.UpdateUserRolePeriod(_context, new BL.Model.DictionaryCore.InternalModel.InternalDictionaryPositionExecutor(Model));

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