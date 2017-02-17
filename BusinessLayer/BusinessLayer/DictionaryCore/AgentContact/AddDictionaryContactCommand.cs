using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;

namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryContactCommand : BaseDictionaryContactCommand
    {
        private AddAgentContact Model { get { return GetModel<AddAgentContact>(); } }

        public override object Execute()
        {
            var newContact = new InternalDictionaryContact(Model);

            CommonDocumentUtilities.SetLastChange(_context, newContact);

            return _dictDb.AddContact(_context, newContact);
        }
    }
}
