using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;

using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.SystemCore;
using System.Collections.Generic;

namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryRegistrationJournalCommand : BaseDictionaryRegistrationJournalCommand
    {
        private AddRegistrationJournal Model { get { return GetModel<AddRegistrationJournal>(); } }

        public override object Execute()
        {
            try
            {
                var model = new InternalDictionaryRegistrationJournal(Model);

                CommonDocumentUtilities.SetLastChange(_context, model);

                return _dictDb.AddRegistrationJournal(_context, model);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}