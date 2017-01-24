using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.InternalModel;

using BL.Model.Exception;
using BL.Model.SystemCore;
using System.Transactions;
using BL.Model.DictionaryCore.FilterModel;
using System.Collections.Generic;
using BL.Model.Enums;
using System.Linq;
using BL.CrossCutting.Helpers;

namespace BL.Logic.DictionaryCore
{
    public class DeleteDictionaryDepartmentCommand : BaseDictionaryCommand

    {
      
        private int Model
        {
            get
            {
                if (!(_param is int))
                {
                    throw new WrongParameterTypeError();
                }
                return (int)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }


        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);
            return true;
        }

        public override object Execute()
        {
            try
            {
                using (var transaction = Transactions.GetTransaction())
                {
                    var frontObj = _dictDb.GetDepartments(_context, new FilterDictionaryDepartment { IDs = new List<int> { Model } }).FirstOrDefault();
                    _logger.Information(_context, null, (int)EnumObjects.DictionaryDepartments, (int)CommandType, frontObj.Id,  frontObj);

                    _dictDb.DeleteDepartments(_context, new List<int>() { Model });
                    transaction.Complete();
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeDeleted(ex);
            }
        }
    }

}

