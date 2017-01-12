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
using BL.CrossCutting.Helpers;

namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryDepartmentCommand : BaseDictionaryDepartmentCommand
    {
        private ModifyDepartment Model { get { return GetModel<ModifyDepartment>(); } }

        public override object Execute()
        {
            try
            {
                var model = new InternalDictionaryDepartment(Model);

                CommonDocumentUtilities.SetLastChange(_context, model);

                using (var transaction = Transactions.GetTransaction())
                {
                    if (string.IsNullOrEmpty(model.Code)) model.Code = GetCode();

                    _dictDb.UpdateDepartment(_context, model);

                    UpdateCodeForChildDepartment(model.Id, model.Code);

                    var frontObj = _dictDb.GetDepartments(_context, new FilterDictionaryDepartment { IDs = new List<int> { model.Id } }).FirstOrDefault();
                    _logger.Information(_context, null, (int)EnumObjects.DictionaryDepartments, (int)CommandType, frontObj.Id, frontObj);

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