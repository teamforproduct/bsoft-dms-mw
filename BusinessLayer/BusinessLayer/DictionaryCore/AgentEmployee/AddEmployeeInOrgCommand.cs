using BL.CrossCutting.Helpers;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System.Linq;

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
            employee.LanguageId

            return true;
        }

        public override object Execute()
        {
            using (var transaction = Transactions.GetTransaction())
            {
                

                e.FirstName = 

                var item = new InternalDictionaryAgentEmployee(e);

                CommonDocumentUtilities.SetLastChange(_context, item);
                int agent = _dictDb.AddAgentEmployee(_context, item);

                if ((item.UserEmail ?? string.Empty) != string.Empty)
                {
                    var contact = new InternalDictionaryContact()
                    {
                        AgentId = agent,
                        ContactTypeId = _dictDb.GetContactsTypeId(_context, EnumContactTypes.MainEmail),
                        Value = item.UserEmail,
                        IsActive = true
                    };
                    CommonDocumentUtilities.SetLastChange(_context, contact);
                    _dictDb.AddContact(_context, contact);
                }

                if ((item.Phone ?? string.Empty) != string.Empty)
                {
                    var contact = new InternalDictionaryContact()
                    {
                        AgentId = agent,
                        ContactTypeId = _dictDb.GetContactsTypeId(_context, EnumContactTypes.MainPhone),
                        Value = item.Phone,
                        IsActive = true
                    };
                    CommonDocumentUtilities.SetLastChange(_context, contact);
                    _dictDb.AddContact(_context, contact);
                }

                transaction.Complete();

                return agent;
            }
        }
    }
}
