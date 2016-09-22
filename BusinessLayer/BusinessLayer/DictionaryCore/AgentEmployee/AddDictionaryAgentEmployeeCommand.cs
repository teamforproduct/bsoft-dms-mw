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
using BL.Model.Enums;

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
                //FirstNameExact = Model.FirstName,
                NameExact = Model.Name,
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
                int agent =_dictDb.AddAgentEmployee(_context, item);

                if ((item.Login ?? string.Empty) != string.Empty)
                {
                    var contact = new InternalDictionaryContact()
                    {
                        AgentId = agent,
                        ContactTypeId = _dictDb.GetContactsTypeId(_context, EnumContactTypes.MainEmail),
                        Value = item.Login,
                        IsActive = true
                    };
                    CommonDocumentUtilities.SetLastChange(_context, contact);
                    _dictDb.AddContact(_context, contact);
                }

                if ((item.Phone ?? string.Empty) != string.Empty)
                {
                    var contact = new InternalDictionaryContact()
                    {
                        AgentId = agent,
                        ContactTypeId = _dictDb.GetContactsTypeId(_context, EnumContactTypes.MainPhone),
                        Value = item.Phone,
                        IsActive = true
                    };
                    CommonDocumentUtilities.SetLastChange(_context, contact);
                    _dictDb.AddContact(_context, contact);
                }

                return agent;
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}
