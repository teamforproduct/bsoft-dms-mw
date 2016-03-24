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

namespace BL.Logic.DictionaryCore.AgentPerson 
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
            var agents = _dictDb.GetDictionaryAgentPersons(_context, new FilterDictionaryAgentPerson
            {
                TaxCode = Model.TaxCode
            },null);
            
            if (agents.Count() > 0)
            {
                throw new DictionaryRecordNotUnique();
            }

            return true;
        }

        public override object Execute()
        {
            try
            {
                var newPerson = new InternalDictionaryAgentPerson
                {
                    Id=Model.Id,
                    FirstName = Model.FirstName,
                    LastName=Model.LastName,
                    MiddleName=Model.MiddleName,
                    TaxCode=Model.TaxCode,
                    IsMale=Model.IsMale,
                    PassportDate=Model.PassportDate,
                    PassportNumber=Model.PassportNumber,
                    PassportSerial=Model.PassportSerial,
                    PassportText=Model.PassportText,
                    Description=Model.Description,
                    BirthDate=Model.BirthDate,
                    IsActive = Model.IsActive
                };
                CommonDocumentUtilities.SetLastChange(_context, newPerson);
                return _dictDb.AddDictionaryAgentPerson(_context, newPerson);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }

    }
}
