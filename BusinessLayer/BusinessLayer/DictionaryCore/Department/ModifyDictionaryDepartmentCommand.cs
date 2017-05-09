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
    public class ModifyDictionaryDepartmentCommand : BaseDictionaryDepartmentCommand
    {
        private ModifyDepartment Model { get { return GetModel<ModifyDepartment>(); } }

        public override object Execute()
        {
            var model = new InternalDictionaryDepartment(Model);

            CommonDocumentUtilities.SetLastChange(_context, model);

            using (var transaction = Transactions.GetTransaction())
            {
                var cp = GetCodePath();

                model.Code = cp.Code;
                model.Path = cp.Path;

                _dictDb.UpdateDepartment(_context, model);

                UpdateCodeForChildDepartment(model.Id, model.Code, model.Path + (string.IsNullOrEmpty(model.Path) ? string.Empty : "/") + model.Id.ToString());

                var frontObj = _dictDb.GetDepartments(_context, new FilterDictionaryDepartment { IDs = new List<int> { model.Id } }).FirstOrDefault();
                _logger.Information(_context, null, (int)EnumObjects.DictionaryDepartments, (int)CommandType, frontObj.Id, frontObj);

                transaction.Complete();
            }
            return null;
        }
    }
}