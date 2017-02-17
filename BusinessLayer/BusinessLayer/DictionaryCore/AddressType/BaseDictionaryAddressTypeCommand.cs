using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;

namespace BL.Logic.DictionaryCore
{
    public class BaseDictionaryAddressTypeCommand : BaseDictionaryCommand
    {
        private AddAddressType Model { get { return GetModel<AddAddressType>(); } }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false, true);

            Model.Code?.Trim();
            Model.Name?.Trim();



            var filter = new FilterDictionaryAddressType
            {
                CodeExact = Model.Code,
            };

            if (TypeModelIs<ModifyAddressType>())
            { filter.NotContainsIDs = new List<int> { GetModel<ModifyAddressType>().Id }; }

            var spr = _dictDb.GetInternalDictionaryAddressType(_context, filter);

            if (spr != null) throw new DictionaryAddressTypeCodeNotUnique(Model.Code);




            filter = new FilterDictionaryAddressType
            {
                NameExact = Model.Name,
            };

            if (TypeModelIs<ModifyAddressType>())
            { filter.NotContainsIDs = new List<int> { GetModel<ModifyAddressType>().Id }; }

            spr = _dictDb.GetInternalDictionaryAddressType(_context, filter);

            if (spr != null) throw new DictionaryAddressTypeNameNotUnique(Model.Name);

            return true;
        }

        public override object Execute()
        { throw new NotImplementedException(); }
    }
}