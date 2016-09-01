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
    public class AddDictionaryAgentEmployeeCommand : BaseDictionaryCommand
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
            _admin.VerifyAccess(_context, CommandType, false, true);
            var agents = _dictDb.GetAgentEmployees(_context, new FilterDictionaryAgentEmployee
            {
                PersonnelNumber = Model.PersonnelNumber,
                TaxCode = Model.TaxCode,
                FirstNameExact = Model.FirstName,
                LastNameExact = Model.LastName,
                PassportSerial = Model.PassportSerial,
                PassportNumber = Model.PassportNumber
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
                return _dictDb.AddAgentEmployee(_context, item);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}
