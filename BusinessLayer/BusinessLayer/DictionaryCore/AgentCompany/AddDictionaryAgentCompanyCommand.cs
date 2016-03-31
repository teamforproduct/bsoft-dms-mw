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

namespace BL.Logic.DictionaryCore.AgentCompany
{
    public class AddDictionaryAgentCompanyCommand : BaseDictionaryCommand
    {
        private ModifyDictionaryAgentCompany Model
        {
            get
            {
                if (!(_param is ModifyDictionaryAgentCompany))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDictionaryAgentCompany)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType, false, true);
            var agents = _dictDb.GetDictionaryAgentCompanies(_context, new FilterDictionaryAgentCompany
            {
                TaxCode = Model.TaxCode,
                OKPOCode=Model.OKPOCode,
                VATCode=Model.VATCode
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
                var newCompany = new InternalDictionaryAgentCompany(Model);;
                CommonDocumentUtilities.SetLastChange(_context, newCompany);
                return _dictDb.AddDictionaryAgentCompany(_context, newCompany);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}
