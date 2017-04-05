using BL.CrossCutting.Helpers;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using System.Linq;

namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryAgentEmployeeCommand : BaseDictionaryAgentEmployeeCommand
    {
        private AddAgentEmployeeUser Model { get { return GetModel<AddAgentEmployeeUser>(); } }

        public override object Execute()
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var item = new InternalDictionaryAgentEmployee(Model);

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
