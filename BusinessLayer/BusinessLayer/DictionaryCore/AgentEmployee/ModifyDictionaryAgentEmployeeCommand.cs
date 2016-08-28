using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using System.Collections.Generic;
using System.Linq;

namespace BL.Logic.DictionaryCore
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

            var agents = _dictDb.GetAgentEmployees(_context, new FilterDictionaryAgentEmployee
            {
                PersonnelNumber = Model.PersonnelNumber,
                TaxCode = Model.TaxCode,
                IsActive=Model.IsActive,
                FirstNameExact = Model.FirstName,
                LastNameExact = Model.LastName,
                PassportSerial = Model.PassportSerial,
                PassportNumber = Model.PassportNumber,
                NotContainsIDs = new List<int> { Model.Id }
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
                var item = new InternalDictionaryAgentEmployee(Model);
                // Обрезаю время для даты рождения и даты получения паспорта
                if (item.PassportDate != null) item.PassportDate = new DateTime(item.PassportDate?.Year ?? 0, item.PassportDate?.Month ?? 0, item.PassportDate?.Day ?? 0);

                if (item.BirthDate != null) item.BirthDate = new DateTime(item.BirthDate?.Year ?? 0, item.BirthDate?.Month ?? 0, item.BirthDate?.Day ?? 0);

                CommonDocumentUtilities.SetLastChange(_context, item);
                _dictDb.UpdateAgentEmployee(_context, item);
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
