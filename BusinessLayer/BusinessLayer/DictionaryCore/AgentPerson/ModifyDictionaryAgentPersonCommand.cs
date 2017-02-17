using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;

namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryAgentPersonCommand : BaseDictionaryAgentPersonCommand
    {
        private ModifyAgentPerson Model { get { return GetModel<ModifyAgentPerson>(); } }

        public override object Execute()
        {
            var newPerson = new InternalDictionaryAgentPerson(Model);
            CommonDocumentUtilities.SetLastChange(_context, newPerson);
            _dictDb.UpdateAgentPerson(_context, newPerson);
            return null;
        }


    }
}
