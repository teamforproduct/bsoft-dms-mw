using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;

namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryRegistrationJournalCommand : BaseDictionaryRegistrationJournalCommand
    {
        private AddRegistrationJournal Model { get { return GetModel<AddRegistrationJournal>(); } }

        public override object Execute()
        {
            var model = new InternalDictionaryRegistrationJournal(Model);

            CommonDocumentUtilities.SetLastChange(_context, model);

            return _dictDb.AddRegistrationJournal(_context, model);
        }
    }
}