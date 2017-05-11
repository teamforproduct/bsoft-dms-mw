using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BL.Logic.DictionaryCore
{
    public class BaseDictionaryDepartmentCommand : BaseDictionaryCommand
    {

        private AddDepartment Model { get { return GetModel<AddDepartment>(); } }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            Model.Name?.Trim();

            if (TypeModelIs<ModifyDepartment>())
            {
                // отдел нельзя подчинить сасому себе и (дочерним отделам)
                if ((Model.ParentId ?? -1) == GetModel<ModifyDepartment>().Id)
                {
                    throw new DictionarysdDepartmentNotBeSubordinated(Model.Name);
                }
            }

            var filter = new FilterDictionaryDepartment
            {
                NameExact = Model.Name,
            };

            if (TypeModelIs<ModifyDepartment>())
            { filter.NotContainsIDs = new List<int> { GetModel<ModifyDepartment>().Id }; }

            if (_dictDb.ExistsDictionaryDepartment(_context, filter)) throw new DictionaryDepartmentNameNotUnique(Model.Name);

            return true;
        }


        public override object Execute()
        { throw new NotImplementedException(); }

        protected CodePath GetCodePath()
        {
            string code = string.Empty;
            string path = string.Empty;

            if ((Model.ParentId ?? 0) > 0)
            {
                var parentDepartment = _dictDb.GetInternalDepartments(_context, new FilterDictionaryDepartment() { IDs = new List<int> { Model.ParentId ?? 0 } }).FirstOrDefault();
                code = parentDepartment.Code;
                path = parentDepartment.Path;
            }

            return new CodePath
            {
                Code = code + (code == string.Empty ? string.Empty : "/") + Model.Index,
                Path = path + (path == string.Empty ? string.Empty : "/") + Model.ParentId?.ToString()
            };
        }

        protected class CodePath
        {
            public string Code { set; get; }

            public string Path { set; get; }
        }

        protected void UpdateCodeForChildDepartment(int departmentId, string codePrefix, string pathPrefix)
        {
            var filter = new FilterDictionaryDepartment() { ParentIDs = new List<int> { departmentId } };

            var childDepartments = _dictDb.GetInternalDepartments(_context, filter);

            if (childDepartments.Count() == 0) return;

            _dictDb.UpdateDepartmentCode(_context, codePrefix, pathPrefix, new FilterDictionaryDepartment() { ParentIDs = new List<int> { departmentId } });

            childDepartments = _dictDb.GetInternalDepartments(_context, filter);

            foreach (var dep in childDepartments)
            {
                UpdateCodeForChildDepartment(dep.Id, dep.Code, dep.Path + (string.IsNullOrEmpty(dep.Path) ? string.Empty : "/") + dep.Id.ToString());
            }

        }
    }
}