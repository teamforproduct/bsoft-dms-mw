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
    public class BaseCustomDictionaryCommand : BaseDictionaryCommand
    {
        private AddCustomDictionary Model { get { return GetModel<AddCustomDictionary>(); } }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            if (!string.IsNullOrEmpty(Model.Code))
            {
                Model.Code?.Trim();

                var filter = new FilterCustomDictionary
                {
                    TypeId = Model.TypeId,
                    CodeExact = Model.Code
                };

                if (TypeModelIs<ModifyCustomDictionary>())
                { filter.NotContainsIDs = new List<int> { GetModel<ModifyCustomDictionary>().Id }; }

                var cd = _dictDb.GetInternalCustomDictionarys(_context, filter).FirstOrDefault();

                if (cd != null)
                {
                    throw new DictionaryCostomDictionaryNotUnique();
                }
            }

            return true;
        }

        public override object Execute()
        { throw new NotImplementedException(); }
    }
}