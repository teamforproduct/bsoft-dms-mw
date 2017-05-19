using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;

namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryAgentPersonExistingCommand : BaseDictionaryCommand
    {
        private AddAgentPersonExisting Model { get { return GetModel<AddAgentPersonExisting>(); } }

        public override object Execute()
        {
            var newPerson = new InternalDictionaryAgentPerson(Model);
            CommonDocumentUtilities.SetLastChange(_context, newPerson);
            _dictDb.AddAgentPersonToCompany(_context, newPerson);
            return null;
        }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute() => true;

    }
}
