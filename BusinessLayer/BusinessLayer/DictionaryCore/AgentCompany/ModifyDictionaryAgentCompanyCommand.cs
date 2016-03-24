using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using System.Collections.Generic;
using System.Linq;

namespace BL.Logic.DictionaryCore.AgentCompany
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
            _admin.VerifyAccess(_context, CommandType, false, true);
            var agents = _dictDb.GetDictionaryAgentCompanies(_context, new FilterDictionaryAgentCompany
            {
                TaxCode = Model.TaxCode,
                OKPOCode = Model.OKPOCode,
                VATCode = Model.VATCode,
                IsActive=Model.IsActive,
                NotContainsId=new List<int> { Model.Id}
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
                var newCompany = new InternalDictionaryAgentCompany
                {

                    Id = Model.Id,
                    FullName = Model.FullName,
                    TaxCode = Model.TaxCode,
                    Description = Model.Description,
                    OKPOCode = Model.OKPOCode,
                    VATCode = Model.VATCode,
                    ContactsPersonsId = Model.ContactsPersonsId,
                    IsActive = Model.IsActive
                };
                CommonDocumentUtilities.SetLastChange(_context, newCompany);
                _dictDb.UpdateDictionaryAgentCompany(_context, newCompany);
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
