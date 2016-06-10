using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using System.Collections.Generic;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.SystemCore;
using System.Linq;

namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryAgentPersonCommand : BaseDictionaryCommand
    {
        private ModifyDictionaryAgentPerson Model
        {
            get
            {
                if (!(_param is ModifyDictionaryAgentPerson))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDictionaryAgentPerson)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType, false, true);
            var agents = _dictDb.GetAgentPersons(_context, new FilterDictionaryAgentPerson
            {
                TaxCode = Model.TaxCode, FirstNameExact = Model.FirstName, LastNameExact = Model.LastName,PassportSerial = Model.PassportSerial, PassportNumber = Model.PassportNumber
            },null);
            
            if (agents.Any())
            {
                throw new DictionaryRecordNotUnique();
            }

            return true;
        }

        public override object Execute()
        {
            try
            {
                var newPerson = new InternalDictionaryAgentPerson(Model);
                CommonDocumentUtilities.SetLastChange(_context, newPerson);
                return _dictDb.AddAgentPerson(_context, newPerson);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }

    }
}
