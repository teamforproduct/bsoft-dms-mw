using System;
using System.Collections.Generic;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;

namespace BL.Logic.DictionaryCore
{
    public class BaseDictionaryContactTypeCommand : BaseDictionaryCommand
    {
        private AddContactType Model { get { return GetModel<AddContactType>(); } }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType,false,true);

            Model.Code?.Trim();
            Model.Name?.Trim();

            //if (dictDb.ExistsContactTypeSpecCode(context, Model.Id)) throw new ();

            var filter = new FilterDictionaryContactType
            {
                NameExact = Model.Name,
            };

            if (TypeModelIs<ModifyContactType>())
            { filter.NotContainsIDs = new List<int> { GetModel<ModifyContactType>().Id }; }

            var spr = _dictDb.GetInternalDictionaryContactType(_context, filter);

            if (spr != null) throw new DictionaryContactTypeNameNotUnique(Model.Name);



            filter = new FilterDictionaryContactType
            {
                CodeExact = Model.Code,
            };

            if (TypeModelIs<ModifyContactType>())
            { filter.NotContainsIDs = new List<int> { GetModel<ModifyContactType>().Id }; }

            spr = _dictDb.GetInternalDictionaryContactType(_context, filter);
            if (spr != null) throw new DictionaryContactTypeNameNotUnique(Model.Code);

            return true;
        }

        public override object Execute()
        { throw new NotImplementedException(); }
    }
}
