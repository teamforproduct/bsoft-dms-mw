using BL.CrossCutting.Helpers;
using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using System.Collections.Generic;

namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryDepartmentCommand : BaseDictionaryDepartmentCommand
    {
        private AddDepartment Model { get { return GetModel<AddDepartment>(); } }

        public override object Execute()
        {
            var model = new InternalDictionaryDepartment(Model);

            CommonDocumentUtilities.SetLastChange(_context, model);

            using (var transaction = Transactions.GetTransaction())
            {
                if (string.IsNullOrEmpty(model.Code)) model.Code = GetCode();

                var id = _dictDb.AddDepartment(_context, model);

                var frontObj = _dictDb.GetDepartment(_context, new FilterDictionaryDepartment { IDs = new List<int> { id } });
                _logger.Information(_context, null, (int)EnumObjects.DictionaryDepartments, (int)CommandType, frontObj.Id, frontObj);

                transaction.Complete();
                return id;
            }

        }
    }
}