using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.SystemCore;
using System.Collections.Generic;

namespace BL.Logic.DictionaryCore
{
    public class BaseDictionaryAddressTypeCommand : BaseDictionaryCommand
    {
       
        protected ModifyDictionaryAddressType Model
        {
            get
            {
                if (!(_param is ModifyDictionaryAddressType))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDictionaryAddressType)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType,false,true);

            Model.Code?.Trim();
            Model.Name?.Trim();

            var spr = _dictDb.GetInternalDictionaryAddressType(_context, new FilterDictionaryAddressType
            {
                CodeExact = Model.Code,
                NotContainsIDs = new List<int> { Model.Id }
            });
            if (spr != null)
            {
                throw new DictionaryAddressTypeCodeNotUnique(Model.Code);
            }

            spr = _dictDb.GetInternalDictionaryAddressType(_context, new FilterDictionaryAddressType
            {
                NameExact = Model.Name,
                NotContainsIDs = new List<int> { Model.Id }
            });
            if (spr != null)
            {
                throw new DictionaryAddressTypeNameNotUnique(Model.Name);
            }

            return true;
        }

        public override object Execute()
        { throw new NotImplementedException(); }
    }
}