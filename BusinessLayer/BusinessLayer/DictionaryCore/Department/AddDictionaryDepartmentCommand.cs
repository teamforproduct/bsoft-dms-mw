using BL.CrossCutting.Helpers;
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
    public class AddDictionaryDepartmentCommand : BaseDictionaryDepartmentCommand
    {

        public override object Execute()
        {
            try
            {
                var dds = CommonDictionaryUtilities.DepartmentModifyToInternal(_context, Model);

                using (var transaction = Transactions.GetTransaction())
                {
                    if (string.IsNullOrEmpty(dds.Code)) dds.Code = GetCode();

                    var id = _dictDb.AddDepartment(_context, dds);

                    var frontObj = _dictDb.GetDepartment(_context, new FilterDictionaryDepartment { IDs = new List<int> { id } });
                    _logger.Information(_context, null, (int)EnumObjects.DictionaryDepartments, (int)CommandType, frontObj.Id, frontObj);

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