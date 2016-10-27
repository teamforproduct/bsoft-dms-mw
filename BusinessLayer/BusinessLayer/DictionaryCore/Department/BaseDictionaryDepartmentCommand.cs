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

        protected ModifyDictionaryDepartment Model
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

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            Model.Name?.Trim();

            // отдел нельзя подчинить сасому себе и (дочерним отделам)
            if ((Model.ParentId ?? -1) == Model.Id)
            {
                throw new DictionarysdDepartmentNotBeSubordinated(Model.Name);
            }

            if (_dictDb.ExistsDictionaryDepartment(_context, new FilterDictionaryDepartment
            {
                NameExact = Model.Name,
                NotContainsIDs = new List<int> { Model.Id }
            }))
            { throw new DictionaryDepartmentNameNotUnique(Model.Name); }

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
                UpdateCodeForChildDepartment( dep.Id, dep.Code);
            }

        }
    }
}