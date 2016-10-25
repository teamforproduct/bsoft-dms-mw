using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System;
using System.Collections.Generic;
using System.Transactions;

namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryDepartmentCommand : BaseDictionaryCommand
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

                    var id = _dictDb.AddDepartment(_context, dds);

                    var frontObj = _dictDb.GetDepartment(_context, new FilterDictionaryDepartment { IDs = new List<int> { id } });
                    _logger.Information(_context, null, (int)EnumObjects.DictionaryDepartments, (int)CommandType, frontObj);

                    transaction.Complete();
                    return id;
                }

            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}