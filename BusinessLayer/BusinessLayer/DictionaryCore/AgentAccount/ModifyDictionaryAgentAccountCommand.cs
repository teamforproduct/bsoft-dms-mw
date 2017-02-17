using BL.Logic.Common;
using BL.Model.DictionaryCore.InternalModel;

namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryAgentAccountCommand : BaseDictionaryAgentAccountCommand
    {
        public override object Execute()
        {
            var newAccount = new InternalDictionaryAgentAccount(Model);
            CommonDocumentUtilities.SetLastChange(_context, newAccount);
            _dictDb.UpdateAgentAccount(_context, newAccount);

            base.Execute();

            return Model.Id;
        }

    }
}
