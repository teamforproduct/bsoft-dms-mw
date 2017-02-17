using BL.Logic.Common;
using BL.Model.DictionaryCore.InternalModel;


namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryAgentCommand : BaseDictionaryAgentCommand
    {
        public override object Execute()
        {
            var newAgent = new InternalDictionaryAgent(Model);
            CommonDocumentUtilities.SetLastChange(_context, newAgent);
            return _dictDb.AddAgent(_context, newAgent);
        }
    }
}
