﻿using BL.Logic.Common;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using System;

namespace BL.Logic.DictionaryCore
{
    public class DeleteDictionaryCompanyCommand : BaseDictionaryCommand

    {
      
        private int Model
        {
            get
            {
                if (!(_param is int))
                {
                    throw new WrongParameterTypeError();
                }
                return (int)_param;
            }
        }

        public override bool CanBeDisplayed(int CompanyId)
        {
            return true;
        }


        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType, false);
            return true;
        }

        public override object Execute()
        {
            try
            {
                var dd = new InternalDictionaryCompany
                {
                    Id = Model

                };
                _dictDb.DeleteCompany(_context, dd);
                return null;
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeDeleted(ex);
            }
        }
    }

}
