using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;


namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryRegistrationJournalCommand : BaseDictionaryRegistrationJournalCommand
    {
        private ModifyRegistrationJournal Model { get { return GetModel<ModifyRegistrationJournal>(); } }
        public override object Execute()
        {
            var model = new InternalDictionaryRegistrationJournal(Model);

            CommonDocumentUtilities.SetLastChange(_context, model);

            _dictDb.UpdateRegistrationJournal(_context, model);
            return null;
        }
    }
}