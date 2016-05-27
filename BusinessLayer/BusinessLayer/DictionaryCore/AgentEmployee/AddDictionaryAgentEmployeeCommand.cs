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
                TaxCode=Model.TaxCode
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
                var newEmployee = new InternalDictionaryAgentEmployee(Model);
                CommonDocumentUtilities.SetLastChange(_context, newEmployee);
                return _dictDb.AddAgentEmployee(_context, newEmployee);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}
