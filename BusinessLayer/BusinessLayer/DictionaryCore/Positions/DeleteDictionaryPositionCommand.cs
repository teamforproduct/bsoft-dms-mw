using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.InternalModel;

using BL.Model.Exception;
using BL.Model.SystemCore;
using System.Transactions;
using BL.Model.DictionaryCore.FilterModel;
using System.Collections.Generic;
using System.Linq;
using BL.Model.Enums;

namespace BL.Logic.DictionaryCore
{
    public class DeleteDictionaryPositionCommand : BaseDictionaryCommand

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

            // PSS Добавить проверку. Существуют ли события от этой должности

            return true;
        }

        public override object Execute()
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    var frontObj = _dictDb.GetPositions(_context, new FilterDictionaryPosition { IDs = new List<int> { Model } }).FirstOrDefault();
                    _logger.Information(_context, null, (int)EnumObjects.DictionaryPositions, (int)CommandType, frontObj.Id, frontObj);

                    _dictDb.DeletePositions(_context, new List<int> { Model });
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

