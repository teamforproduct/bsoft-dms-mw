using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;

namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryContactCommand : BaseDictionaryContactCommand
    {
        private ModifyAgentContact Model { get { return GetModel<ModifyAgentContact>(); } }

        public override object Execute()
        {
            var newContact = new InternalDictionaryContact(Model);
            CommonDocumentUtilities.SetLastChange(_context, newContact);
            _dictDb.UpdateContact(_context, newContact);
            return null;
        }
    }
}
