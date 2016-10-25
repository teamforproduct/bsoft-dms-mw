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
using BL.Model.Enums;
using System.Linq;

namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryDepartmentCommand : BaseDictionaryCommand
    {

        private ModifyDictionaryDepartment Model
        {
            get
            {
                if (!(_param is ModifyDictionaryDepartment))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDictionaryDepartment)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            DictionaryModelVerifying.VerifyDepartment(_context, _dictDb, Model);

            return true;
        }


        public override object Execute()
        {
            try
            {
                var dds = CommonDictionaryUtilities.DepartmentModifyToInternal(_context, Model);
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    _dictDb.UpdateDepartment(_context, dds);

                    var frontObj = _dictDb.GetDepartments(_context, new FilterDictionaryDepartment { IDs = new List<int> { dds.Id } }).FirstOrDefault();
                    _logger.Information(_context, null, (int)EnumObjects.DictionaryDepartments, (int)CommandType, frontObj);

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