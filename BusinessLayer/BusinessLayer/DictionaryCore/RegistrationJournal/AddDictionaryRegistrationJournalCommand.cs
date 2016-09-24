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
    public class AddDictionaryRegistrationJournalCommand : BaseDictionaryCommand
    {
   
        private ModifyDictionaryRegistrationJournal Model
        {
            get
            {
                if (!(_param is ModifyDictionaryRegistrationJournal))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDictionaryRegistrationJournal)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            DictionaryModelVerifying.VerifyRegistrationJournal(_context, _dictDb, Model);
         
            return true;
        }

        public override object Execute()
        {
            try
            {
                var drj = CommonDictionaryUtilities.RegistrationJournalModifyToInternal(_context, Model);

                return _dictDb.AddRegistrationJournal(_context, drj);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}