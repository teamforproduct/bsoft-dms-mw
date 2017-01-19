using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using System.Linq;
using System.Collections.Generic;

namespace BL.Logic.DictionaryCore
{
    public class BaseCustomDictionaryTypeCommand : BaseDictionaryCommand
    {
        private AddCustomDictionaryType Model { get { return GetModel<AddCustomDictionaryType>(); } }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            Model.Code?.Trim();

            var filter = new FilterCustomDictionaryType
            {
                CodeExact = Model.Code
            };

            if (TypeModelIs<ModifyCustomDictionaryType>())
            { filter.NotContainsIDs = new List<int> { GetModel<ModifyCustomDictionaryType>().Id }; }

            var cdt = _dictDb.GetInternalCustomDictionaryTypes(_context, filter).FirstOrDefault();

            if (cdt != null) throw new DictionaryCostomDictionaryTypeNotUnique(Model.Code);

            return true;
        }

        public override object Execute()
        { throw new NotImplementedException(); }
    }
}