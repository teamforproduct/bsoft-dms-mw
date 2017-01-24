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

        protected string GetCode()
        {
            string code = string.Empty;

            if ((Model.ParentId ?? 0) > 0)
            {
                var parentDepartment = _dictDb.GetDepartment(_context, new FilterDictionaryDepartment() { IDs = new List<int> { Model.ParentId ?? 0 } });
                code = parentDepartment.Code;
            }

            return code + (code == string.Empty ? string.Empty : "/") + Model.Index;
        }

        protected void UpdateCodeForChildDepartment(int departmentId, string codePrefix)
        {
            var childDepartments = _dictDb.GetDepartments(_context, new FilterDictionaryDepartment() { ParentIDs = new List<int> { departmentId } });

            if (childDepartments.Count() == 0) return;

            _dictDb.UpdateDepartmentCode(_context, codePrefix, new FilterDictionaryDepartment() { ParentIDs = new List<int> { departmentId } });

            foreach (var dep in childDepartments)
            {
                UpdateCodeForChildDepartment(dep.Id, dep.Code);
            }

        }
    }
}