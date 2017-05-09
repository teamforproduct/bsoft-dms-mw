using BL.CrossCutting.Helpers;
using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using System.Collections.Generic;
using System.Linq;

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
                var cp = GetCodePath();

                model.Code = cp.Code;
                model.Path = cp.Path;

                var id = _dictDb.AddDepartment(_context, model);

                var frontObj = _dictDb.GetInternalDepartments(_context, new FilterDictionaryDepartment { IDs = new List<int> { id } }).FirstOrDefault();
                _logger.Information(_context, null, (int)EnumObjects.DictionaryDepartments, (int)CommandType, frontObj.Id, frontObj);

                transaction.Complete();
                return id;
            }

        }
    }
}