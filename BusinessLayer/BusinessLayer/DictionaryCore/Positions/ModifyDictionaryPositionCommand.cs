using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.SystemCore;
using System.Collections.Generic;
using System.Transactions;
using System.Linq;
using BL.Model.Enums;

namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryPositionCommand : BaseDictionaryCommand
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
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {

                    _dictDb.UpdatePosition(_context, dp);
                    var frontObj = _dictDb.GetDepartments(_context, new FilterDictionaryDepartment { IDs = new List<int> { dp.Id } }).FirstOrDefault();
                    _logger.Information(_context, null, (int)EnumObjects.DictionaryPositions, (int)CommandType, frontObj);

                    _dictService.SetPositionOrder(_context, Model.Id, Model.Order);
                    transaction.Complete();
                }
            }
            catch (DictionaryRecordWasNotFound)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseError(ex);
            }
            return null;
        }
    }
}