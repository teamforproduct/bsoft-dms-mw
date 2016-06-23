﻿using System;
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
            var agents = _dictDb.GetAgentCompanies(_context, new FilterDictionaryAgentCompany
            {
                NameExact = Model.FullName,
                TaxCodeExact = Model.TaxCode,
                OKPOCodeExact=Model.OKPOCode,
                VATCodeExact= Model.VATCode
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
                var newCompany = new InternalDictionaryAgentCompany(Model);;
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
