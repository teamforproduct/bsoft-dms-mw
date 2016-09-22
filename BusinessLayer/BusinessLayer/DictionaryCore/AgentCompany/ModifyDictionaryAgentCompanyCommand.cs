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
    public class ModifyDictionaryAgentCompanyCommand : BaseDictionaryCommand
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
            _admin.VerifyAccess(_context, CommandType, false);

            if (_dictDb.ExistsAgents(_context, new FilterDictionaryAgent() { NameExact = Model.Name, NotContainsIDs = new List<int> { Model.Id } }))
            {
                throw new DictionaryAgentNameNotUnique();
            }

            if (!string.IsNullOrEmpty(Model.TaxCode))
            {
                if (_dictDb.ExistsAgentCompanies(_context, new FilterDictionaryAgentCompany()
                {
                    TaxCodeExact = Model.TaxCode,
                    NotContainsIDs = new List<int> { Model.Id }
                }))
                { throw new DictionaryAgentCompanyTaxCodeNotUnique(); }
            }

            if (!string.IsNullOrEmpty(Model.OKPOCode))
            {
                if (_dictDb.ExistsAgentCompanies(_context, new FilterDictionaryAgentCompany()
                {
                    OKPOCodeExact = Model.OKPOCode,
                    NotContainsIDs = new List<int> { Model.Id }
                }))
                { throw new DictionaryAgentCompanyOKPOCodeNotUnique(); }
            }

            if (!string.IsNullOrEmpty(Model.VATCode))
            {
                if (_dictDb.ExistsAgentCompanies(_context, new FilterDictionaryAgentCompany()
                {
                    VATCodeExact = Model.VATCode,
                    NotContainsIDs = new List<int> { Model.Id }
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
                _dictDb.UpdateAgentCompany(_context, newCompany);
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
