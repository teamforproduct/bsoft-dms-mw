using BL.Logic.Common;
using BL.Model.DictionaryCore.InternalModel;


namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryAgentAccountCommand : BaseDictionaryAgentAccountCommand
    {
        public override object Execute()
        {
            var newAccount = new InternalDictionaryAgentAccount(Model);
            CommonDocumentUtilities.SetLastChange(_context, newAccount);
            int account = _dictDb.AddAgentAccount(_context, newAccount);

            base.Execute();

            return account;
        }
    }
}
