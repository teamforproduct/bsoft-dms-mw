using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using System.Collections.Generic;
using System.Linq;

namespace BL.Logic.DictionaryCore.AgentEmployee
{
    public class ModifyDictionaryAgentEmployeeCommand :BaseDictionaryCommand
    {
        private ModifyDictionaryAgentEmployee Model
        {
            get
            {
                if (!(_param is ModifyDictionaryAgentEmployee))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDictionaryAgentEmployee)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType, false);

            var agents = _dictDb.GetDictionaryAgentEmployees(_context, new FilterDictionaryAgentEmployee
            {
                PersonnelNumber = Model.PersonnelNumber,
                TaxCode = Model.TaxCode,
                IsActive=Model.IsActive,
                NotContainsId=new List<int> { Model.Id}
            });

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
                var newPerson = new InternalDictionaryAgentEmployee
                {
                    Id = Model.Id,
                    PersonnelNumber=Model.PersonnelNumber,
                    FirstName = Model.FirstName,
                    LastName = Model.LastName,
                    MiddleName = Model.MiddleName,
                    TaxCode = Model.TaxCode,
                    IsMale = Model.IsMale,
                    PassportSerial = Model.PassportSerial,
                    PassportNumber = Model.PassportNumber,
                    PassportText = Model.PassportText,
                    PassportDate = Model.PassportDate,
                    IsActive = Model.IsActive,
                    BirthDate = Model.BirthDate,
                    Description = Model.Description
                };
                CommonDocumentUtilities.SetLastChange(_context, newPerson);
                _dictDb.UpdateDictionaryAgentEmployee(_context, newPerson);
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
