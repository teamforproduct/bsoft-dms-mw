using BL.CrossCutting.Helpers;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System;

namespace BL.Logic.DictionaryCore
{
    public class AddEmployeeInOrgCommand : BaseDictionaryCommand
    {
        private AddEmployeeInOrg Model { get { return GetModel<AddEmployeeInOrg>(); } }

        private AddAgentEmployeeUser employee = new AddAgentEmployeeUser();

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            if (Model.OrgId == null && string.IsNullOrEmpty(Model.OrgName)) throw new OrgRequired();

            if (Model.DepartmentId == null && string.IsNullOrEmpty(Model.DepartmentName)) throw new DepartmentRequired();

            if (Model.PositionId == null && string.IsNullOrEmpty(Model.PositionName)) throw new PositionRequired();

            employee.Login = Model.Login;
            employee.FirstName = Model.FirstName;
            employee.MiddleName = Model.MiddleName;
            employee.LastName = Model.LastName;
            employee.IsActive = true;
            employee.LanguageId = Model.LanguageId;

            return true;
        }

        public override object Execute()
        {

            using (var transaction = Transactions.GetTransaction())
            {

                if (Model.OrgId == null && !string.IsNullOrEmpty(Model.OrgName))
                {
                    var org = new AddOrg();
                    org.FullName = Model.OrgName;
                    org.Name = Model.OrgName;
                    org.IsActive = true;

                    Model.OrgId = (int)_dictService.ExecuteAction(EnumDictionaryActions.AddAgentClientCompany, _context, org);
                }

                if (Model.DepartmentId == null && !string.IsNullOrEmpty(Model.DepartmentName))
                {
                    var dep = new AddDepartment();
                    dep.CompanyId = Model.OrgId ?? -1;
                    dep.FullName = Model.DepartmentName;
                    dep.Name = Model.DepartmentName;
                    dep.IsActive = true;
                    dep.Index = Model.DepartmentIndex;

                    Model.DepartmentId = (int)_dictService.ExecuteAction(EnumDictionaryActions.AddDepartment, _context, dep);
                }

                if (Model.PositionId == null && !string.IsNullOrEmpty(Model.PositionName))
                {
                    var pos = new AddPosition();
                    pos.DepartmentId = Model.DepartmentId ?? -1;
                    pos.FullName = Model.PositionName;
                    pos.Name = Model.PositionName;
                    pos.IsActive = true;

                    Model.PositionId = (int)_dictService.ExecuteAction(EnumDictionaryActions.AddPosition, _context, pos);
                }

                var empId = (int)_dictService.ExecuteAction(EnumDictionaryActions.AddAgentEmployee, _context, employee);

                var ass = new AddPositionExecutor();
                ass.AccessLevelId = EnumAccessLevels.Personally;
                ass.AgentId = empId;
                ass.IsActive = true;
                ass.PositionId = Model.PositionId ?? -1;
                ass.StartDate = DateTime.UtcNow;

                var assignmentId = _dictService.ExecuteAction(EnumDictionaryActions.AddExecutor, _context, ass);

                transaction.Complete();

                return empId;
            }
        }
    }
}
