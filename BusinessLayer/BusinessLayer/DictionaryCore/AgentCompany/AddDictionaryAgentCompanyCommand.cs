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

            if (_dictDb.ExistsAgents(_context, new FilterDictionaryAgent() { NameExact = Model.Name }))
            {
                throw new DictionaryAgentNameNotUnique();
            }

            if (!string.IsNullOrEmpty(Model.TaxCode))
            {
                if (_dictDb.ExistsAgentCompanies(_context, new FilterDictionaryAgentCompany()
                {
                    TaxCodeExact = Model.TaxCode,
                }))
                { throw new DictionaryAgentCompanyTaxCodeNotUnique(); }
            }

            if (!string.IsNullOrEmpty(Model.OKPOCode))
            {
                if (_dictDb.ExistsAgentCompanies(_context, new FilterDictionaryAgentCompany()
                {
                    OKPOCodeExact = Model.OKPOCode,
                }))
                { throw new DictionaryAgentCompanyOKPOCodeNotUnique(); }
            }

            if (!string.IsNullOrEmpty(Model.VATCode))
            {
                if (_dictDb.ExistsAgentCompanies(_context, new FilterDictionaryAgentCompany()
                {
                    VATCodeExact = Model.VATCode
                }))
                { throw new DictionaryAgentCompanyVATCodeNotUnique(); }
            }

            return true;
        }

        public override object Execute()
        {
            try
            {
                var newCompany = new InternalDictionaryAgentCompany(Model); ;
                CommonDocumentUtilities.SetLastChange(_context, newCompany);
                return _dictDb.AddAgentCompany(_context, newCompany);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}
