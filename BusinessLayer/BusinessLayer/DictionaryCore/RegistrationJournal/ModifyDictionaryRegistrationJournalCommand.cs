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
    public class ModifyDictionaryRegistrationJournalCommand : BaseDictionaryRegistrationJournalCommand
    {
        private ModifyRegistrationJournal Model { get { return GetModel<ModifyRegistrationJournal>(); } }
        public override object Execute()
        {
            try
            {
                var model = new InternalDictionaryRegistrationJournal(Model);

                CommonDocumentUtilities.SetLastChange(_context, model);

                _dictDb.UpdateRegistrationJournal(_context, model);
            }
            catch (DictionaryRecordWasNotFound)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseError(ex);
            }
            return null;
        }
    }
}