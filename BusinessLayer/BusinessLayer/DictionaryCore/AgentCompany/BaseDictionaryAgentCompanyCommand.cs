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
    public class BaseDictionaryAgentCompanyCommand : BaseDictionaryCommand
    {
        protected ModifyDictionaryAgentCompany Model
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

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            Model.Name?.Trim();
            Model.TaxCode?.Trim();
            Model.OKPOCode?.Trim();
            Model.VATCode?.Trim();

            if (_dictDb.ExistsAgents(_context, new FilterDictionaryAgent()
            {
                NameExact = Model.Name,
                NotContainsIDs = new List<int> { Model.Id }
            }))
            { throw new DictionaryAgentNameNotUnique(Model.Name); }

            if (!string.IsNullOrEmpty(Model.TaxCode))
            {
                if (_dictDb.ExistsAgentCompanies(_context, new FilterDictionaryAgentCompany()
                {
                    TaxCodeExact = Model.TaxCode,
                    NotContainsIDs = new List<int> { Model.Id }
                }))
                { throw new DictionaryAgentCompanyTaxCodeNotUnique(Model.Name, Model.TaxCode); }
            }

            if (!string.IsNullOrEmpty(Model.OKPOCode))
            {
                if (_dictDb.ExistsAgentCompanies(_context, new FilterDictionaryAgentCompany()
                {
                    OKPOCodeExact = Model.OKPOCode,
                    NotContainsIDs = new List<int> { Model.Id }
                }))
                { throw new DictionaryAgentCompanyOKPOCodeNotUnique(Model.Name, Model.OKPOCode); }
            }

            if (!string.IsNullOrEmpty(Model.VATCode))
            {
                if (_dictDb.ExistsAgentCompanies(_context, new FilterDictionaryAgentCompany()
                {
                    VATCodeExact = Model.VATCode,
                    NotContainsIDs = new List<int> { Model.Id }
                }))
                { throw new DictionaryAgentCompanyVATCodeNotUnique(Model.Name, Model.VATCode); }
            }
            return true;
        }

        public override object Execute()
        { throw new NotImplementedException(); }
    }
}
