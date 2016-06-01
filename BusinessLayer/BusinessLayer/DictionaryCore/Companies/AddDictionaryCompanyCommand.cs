﻿using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;

namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryCompanyCommand : BaseDictionaryCommand
    {

        private ModifyDictionaryCompany Model
        {
            get
            {
                if (!(_param is ModifyDictionaryCompany))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDictionaryCompany)_param;
            }
        }

        public override bool CanBeDisplayed(int CompanyId)
        {
            return true;
        }

        public override bool CanExecute()
        {

            _admin.VerifyAccess(_context, CommandType, false);

            var fdd = new FilterDictionaryCompany { Name = Model.Name, NotContainsIDs = new List<int> { Model.Id } };

            if (_dictDb.ExistsCompany(_context, fdd))
            {
                throw new DictionaryRecordNotUnique();
            }

            return true;
        }

        public override object Execute()
        {
            try
            {
                var dp = CommonDictionaryUtilities.CompanyModifyToInternal(_context, Model);

                return _dictDb.AddCompany(_context, dp);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}