using BL.CrossCutting.Helpers;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Exception;

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

            employee.FirstName = Model.FirstName;
            employee.IsActive = true;
            //employee.LanguageId;

            return true;
        }

        private void AddAgentEmployeeUser(AddAgentEmployeeUser model)
        {

        }

        public override object Execute()
        {

            using (var transaction = Transactions.GetTransaction())
            {

                var agent = _dictService.ExecuteAction(BL.Model.Enums.EnumDictionaryActions.AddAgentEmployee, _context, employee);



                transaction.Complete();

                return agent;
            }
        }
    }
}
